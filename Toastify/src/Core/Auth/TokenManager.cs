using System;
using System.Threading;
using log4net;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Events;
using ToastifyAPI.Helpers;

namespace Toastify.Core.Auth
{
    public sealed class TokenManager : ITokenManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TokenManager));

        private readonly object tokenLock = new object();
        private readonly object refreshTimerLock = new object();

        private bool isGettingToken;

        #region Non-Public Properties

        private ISpotifyWebAuth SpotifyWebAuth { get; }

        private Timer RefreshTimer { get; }

        #endregion

        #region Public Properties

        public IToken Token { get; private set; }
        public ManualResetEvent RefreshingTokenEvent { get; }

        #endregion

        #region Events

        public event EventHandler<SpotifyTokenChangedEventArgs> TokenChanged;
        public event EventHandler TokenReleased;
        public event EventHandler TokenNull;

        #endregion

        public TokenManager(ISpotifyWebAuth spotifyWebAuth)
        {
            this.SpotifyWebAuth = spotifyWebAuth ?? App.Container.Resolve<ISpotifyWebAuth>();
            this.RefreshTimer = new Timer(this.RefreshTimerCallback);
            this.RefreshingTokenEvent = new ManualResetEvent(true);
        }

        public bool BeginGetToken(CancellationToken cancellationToken)
        {
            return this.BeginGetToken(cancellationToken, null);
        }

        public bool BeginGetToken(CancellationToken cancellationToken, Action<IToken> callback)
        {
            if (this.isGettingToken)
                return false;

            this.isGettingToken = true;
            ThreadPool.QueueUserWorkItem(async _ =>
            {
                try
                {
                    logger.Debug("Begin GetToken");

                    CancellationToken ct = (CancellationToken)_;
                    if (ct.IsCancellationRequested)
                        return;

                    IToken token = null;

                    // Check if a local token file exists
                    const string tokenFileName = "spotify-token.sec";
                    if (Security.ProtectedDataExists(tokenFileName))
                    {
                        logger.Debug("Fetching cached token...");

                        IToken savedToken = Security.GetProtectedObject<IToken>(tokenFileName);
                        if (savedToken != null)
                        {
                            if (!savedToken.IsExpired())
                                token = savedToken;
                            else if (!string.IsNullOrWhiteSpace(savedToken.RefreshToken))
                            {
                                logger.Debug("Cached token is expired. Refreshing it...");
                                savedToken = await this.SpotifyWebAuth.RefreshToken(savedToken).ConfigureAwait(false);
                                if (ct.IsCancellationRequested)
                                    return;

                                if (savedToken != null)
                                {
                                    token = savedToken;
                                    Security.SaveProtectedObject(savedToken, tokenFileName);
                                }
                                else
                                {
                                    // Delete the file since the token couldn't be refreshed
                                    Security.DeleteProtectedData(tokenFileName);
                                    logger.Debug("Cached token deleted since it couldn't be refreshed");
                                }
                            }
                            else
                            {
                                // Delete the file since the token is expired and it can't be refreshed
                                Security.DeleteProtectedData(tokenFileName);
                                logger.Debug("Cached token deleted since it was expired and it was not refreshable");
                            }
                        }
                        else
                        {
                            // Delete the file since it isn't a valid IToken file
                            Security.DeleteProtectedData(tokenFileName);
                            logger.Debug("Cached token deleted since it wasn't a valid token file");
                        }
                    }

                    if (token == null)
                    {
                        // Request new token
                        logger.Debug("Requesting new token...");
                        token = await this.SpotifyWebAuth.GetToken().ConfigureAwait(false);
                        if (!ct.IsCancellationRequested && token != null)
                            Security.SaveProtectedObject(token, tokenFileName);
                    }

                    this.isGettingToken = false;

                    if (!ct.IsCancellationRequested)
                    {
                        this.OnTokenChanged(token);
                        callback?.Invoke(token);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Unhandled exception while getting token", e);
                }
                finally
                {
                    this.isGettingToken = false;
                }
            }, cancellationToken);

            return true;
        }

        public void ReleaseToken()
        {
            this.OnTokenChanged(null);
            this.OnTokenReleased();
        }

        private async void RefreshTimerCallback(object state)
        {
            logger.Info("Refreshing access token...");

            if (this.isGettingToken)
                return;

            IToken thisToken;
            lock (this.tokenLock)
            {
                if (this.Token == null)
                    return;
                thisToken = this.Token;
            }

            this.RefreshingTokenEvent.Reset();
            IToken token = await this.SpotifyWebAuth.RefreshToken(thisToken).ConfigureAwait(false);
            this.OnTokenChanged(token);
            this.RefreshingTokenEvent.Set();
        }

        public void Dispose()
        {
            this.RefreshTimer?.Dispose();
            this.RefreshingTokenEvent?.Dispose();
        }

        #region Event Handlers

        private void OnTokenChanged(IToken newToken)
        {
            if (logger.IsDebugEnabled)
            {
                string logMsg = newToken?.GetExpirationInfo() ?? "null";
                logger.Debug($"Token changed: {logMsg}");
            }
            else
                logger.Info("Token changed");

            if (newToken != null && newToken.IsExpired())
                newToken = null;

            // Notify subscribers
            lock (this.tokenLock)
            {
                if (this.Token == null && newToken != null || this.Token != null && !this.Token.Equals(newToken))
                {
                    IToken oldToken = this.Token;
                    this.Token = newToken;
                    this.TokenChanged?.Invoke(this, new SpotifyTokenChangedEventArgs(oldToken, newToken));
                }

                if (this.Token != null && newToken == null)
                    this.OnTokenNull();
            }

            // Reset refresh timer
            if (newToken != null)
            {
                lock (this.refreshTimerLock)
                {
                    TimeSpan dueTime = newToken.CreateDate.Add(TimeSpan.FromSeconds(newToken.ExpiresIn)) - DateTime.Now - TimeSpan.FromMinutes(2);
                    if (dueTime < TimeSpan.Zero)
                        dueTime = TimeSpan.Zero;

                    this.RefreshTimer.Change(dueTime, Timeout.InfiniteTimeSpan);
                }
            }
        }

        private void OnTokenReleased()
        {
            this.TokenReleased?.Invoke(this, EventArgs.Empty);
        }

        private void OnTokenNull()
        {
            this.TokenNull?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}