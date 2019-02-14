using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using log4net;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using Toastify.Threading;
using Toastify.View;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;

namespace Toastify.Core.Auth
{
    public class ToastifyWebAuth : BaseSpotifyWebAuth
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyWebAuth));

        private readonly ManualResetEvent abortAuthEvent;

        private AuthHttpServer authHttpServer;
        private WindowThread<WebView> webViewWindowThread;

        #region Non-Public Properties

        protected override IAuthHttpServer AuthHttpServer
        {
            get { return this.authHttpServer; }
        }

        #endregion

        #region Public Properties

        public override string Scopes { get; }
        public override string State { get; }
        public override bool ShowDialog { get; }

        #endregion

        public ToastifyWebAuth(Scope scopes, string state, bool showDialog)
        {
            this.Scopes = scopes.GetStringAttribute(" ");
            this.State = state;
            this.ShowDialog = showDialog;

            this.authHttpServer = new AuthHttpServer();
            this.abortAuthEvent = new ManualResetEvent(false);
        }

        protected override void Authorize()
        {
            WindowThreadOptions<WebView> windowThreadOptions = new WindowThreadOptions<WebView>
            {
                WindowInitialization = window =>
                {
                    window.Title = "Spotify Authorization";
                    window.AllowedHosts = new List<string> { "accounts.spotify.com", "localhost", string.Empty };
                    window.SetSize(new Size(475, 512));
                },
                BeforeWindowShownAction = window =>
                {
                    window.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        AuthorizationCodeFlow.Authorize(url => window.NavigateTo(url), this.Scopes, this.State, this.ShowDialog, "en");
                    }));
                },
                OnWindowClosingAction = window => this.abortAuthEvent.Set()
            };

            this.webViewWindowThread = ThreadManager.Instance.CreateWindowThread(ApartmentState.STA, windowThreadOptions);
            this.webViewWindowThread.IsBackground = true;
            this.webViewWindowThread.ThreadName = $"{nameof(ToastifyWebAuth)}_{nameof(WebView)}_Thread";
            this.webViewWindowThread.Start();
        }

        public override Task<IToken> GetToken()
        {
            this.abortAuthEvent.Reset();
            return this.authHttpServer == null ? Task.FromResult((IToken)null) : base.GetToken();
        }

        protected override IToken CreateToken(SpotifyTokenResponse spotifyTokenResponse)
        {
            return new Token(spotifyTokenResponse);
        }

        protected override Task<bool> ShouldAbortAuthorization()
        {
            try
            {
                bool shouldAbortAuthorization = this.abortAuthEvent.WaitOne(20) || App.ShutdownEvent.WaitOne(20);
                return Task.FromResult(shouldAbortAuthorization);
            }
            catch
            {
                return Task.FromResult(true);
            }
        }

        protected override void AuthHttpServer_AuthorizationFinished(object sender, AuthEventArgs e)
        {
            try
            {
                base.AuthHttpServer_AuthorizationFinished(sender, e);
                this.DisposeWebViewWindow(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                logger.Error($"Unhandled error in {nameof(this.AuthHttpServer_AuthorizationFinished)}", ex);
            }
        }

        #region Dispose

        public override void Dispose()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(2);
            this.abortAuthEvent.Set();
            this.DisposeAuthHttpServer(timeout);
            this.DisposeWebViewWindow(timeout);
            this.abortAuthEvent?.Dispose();
        }

        private void DisposeAuthHttpServer(TimeSpan timeout)
        {
            this.authHttpServer?.Dispose(timeout);
            this.authHttpServer = null;
        }

        private void DisposeWebViewWindow(TimeSpan timeout)
        {
            this.webViewWindowThread?.Dispose(timeout);
            this.webViewWindowThread = null;
        }

        #endregion
    }
}