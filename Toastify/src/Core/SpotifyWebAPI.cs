using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Aleab.Common.Extensions;
using JetBrains.Annotations;
using log4net;
using SpotifyAPI.Web.Models;
using Toastify.Model;
using ToastifyAPI.Core;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Events;
using ToastifyAPI.Model.Interfaces;
using SpotifyAPIWebAPI = SpotifyAPI.Web.SpotifyWebAPI;

namespace Toastify.Core
{
    public class SpotifyWebAPI : ISpotifyWebAPI
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SpotifyWebAPI));

        private SpotifyAPIWebAPI _spotifyWebApi;
        private IToken _token;

        #region Non-Public Properties

        private SpotifyAPIWebAPI SpotifyWebApi
        {
            get
            {
                return this._spotifyWebApi ?? (this._spotifyWebApi = new SpotifyAPIWebAPI(App.ProxyConfig.ProxyConfig)
                {
                    TokenType = this.Token?.TokenType,
                    AccessToken = this.Token?.AccessToken,
                    UseAuth = true
                });
            }
        }

        private ITokenManager TokenManager { get; }

        #endregion

        #region Public Properties

        public IToken Token
        {
            get { return this._token; }
            set
            {
                if (this._token == null && value != null || this._token != null && !this._token.Equals(value))
                {
                    this._token = value;
                    if (this._spotifyWebApi != null)
                    {
                        this._spotifyWebApi.TokenType = this._token?.TokenType;
                        this._spotifyWebApi.AccessToken = this._token?.AccessToken;
                    }
                }
            }
        }

        #endregion

        public SpotifyWebAPI([NotNull] ITokenManager tokenManager)
        {
            this.TokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
            this.TokenManager.TokenChanged += this.TokenManager_TokenChanged;
        }

        public async Task<ICurrentlyPlayingObject> GetCurrentlyPlayingTrackAsync()
        {
            if (this.SpotifyWebApi == null || !this.WaitForTokenRefresh())
                return null;

            PlaybackContext playbackContext = await this.PerformRequest(
                async () => await this.SpotifyWebApi.GetPlayingTrackAsync().ConfigureAwait(false),
                "Couldn't get the current playback context.").ConfigureAwait(false);

            return playbackContext != null ? new CurrentlyPlayingObject(playbackContext) : null;
        }

        public async Task<ISpotifyUserProfile> GetUserPrivateProfileAsync()
        {
            if (this.SpotifyWebApi == null || !this.WaitForTokenRefresh())
                return null;

            PrivateProfile profile = await this.PerformRequest(
                async () => await this.SpotifyWebApi.GetPrivateProfileAsync().ConfigureAwait(false),
                "Couldn't get the current user's private profile.").ConfigureAwait(false);

            return profile != null ? new SpotifyUserProfile(profile) : null;
        }

        private async Task<T> PerformRequest<T>(Func<Task<T>> request, string errorMsg) where T : BasicModel
        {
            return await this.PerformRequest(request, errorMsg, true).ConfigureAwait(false);
        }

        private async Task<T> PerformRequest<T>(Func<Task<T>> request, string errorMsg, bool retry) where T : BasicModel
        {
            T response = await request();
            if (response == null || response.StatusCode() == HttpStatusCode.NoContent)
                return null;

            LogReturnedValueIfError(errorMsg, response);

            var statusCode = response.StatusCode();
            if (statusCode == (HttpStatusCode)431)
            {
                logger.Debug("HTTP 431 received: a new instance of SpotifyWebApi will be created.");
                this._spotifyWebApi = null;

                // Retry
                if (retry)
                    response = await this.PerformRequest(request, errorMsg, false).ConfigureAwait(false);
            }

            return response;
        }

        private bool WaitForTokenRefresh()
        {
            if (!this.TokenManager.RefreshingTokenEvent.WaitOne(TimeSpan.FromSeconds(30)))
            {
                logger.Warn($"Timeout while waiting for the token to refresh: API request cancelled!{Environment.NewLine}{new StackTrace().GetFrames(1, 2)}");
                return false;
            }

            return true;
        }

        private void TokenManager_TokenChanged(object sender, SpotifyTokenChangedEventArgs e)
        {
            this.Token = e.NewToken;
        }

        #region Static Members

        private static void LogReturnedValueIfError(string msg, BasicModel ret)
        {
            if (ret == null || ret.StatusCode() != HttpStatusCode.OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{msg} Returned value = ");
                if (ret != null)
                {
                    sb.Append($"{{{Environment.NewLine}")
                      .Append($"   StatusCode: \"{ret.StatusCode()}\"");
                    if (ret.HasError())
                    {
                        sb.Append($",{Environment.NewLine}")
                          .Append($"   Error: {{{Environment.NewLine}")
                          .Append($"      Status: {ret.Error.Status},{Environment.NewLine}")
                          .Append($"      Message: \"{ret.Error.Message}\"{Environment.NewLine}")
                          .Append("   }");
                    }

                    sb.Append($"{Environment.NewLine}}}");
                }
                else
                    sb.Append("null");

                logger.Warn(sb.ToString());
            }
        }

        #endregion
    }
}