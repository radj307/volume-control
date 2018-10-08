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
                OnWindowClosingAction = windowThread => windowThread.Abort()
            };

            this.webViewWindowThread = ThreadManager.Instance.CreateWindowThread(ApartmentState.STA, windowThreadOptions);
            this.webViewWindowThread.IsBackground = true;
            this.webViewWindowThread.ThreadName = $"{nameof(ToastifyWebAuth)}_{nameof(WebView)}_Thread";
            this.webViewWindowThread.Start();
        }

        public override Task<IToken> GetToken()
        {
            return this.authHttpServer == null ? Task.FromResult((IToken)null) : base.GetToken();
        }

        protected override IToken CreateToken(SpotifyTokenResponse spotifyTokenResponse)
        {
            return new Token(spotifyTokenResponse);
        }

        protected override Task<bool> ShouldAbortAuthorization()
        {
            bool shouldAbortAuthorization = App.ShutdownEvent.WaitOne(100);
            return Task.FromResult(shouldAbortAuthorization);
        }

        public override void Dispose()
        {
            this.authHttpServer?.Dispose();
            this.authHttpServer = null;

            this.webViewWindowThread?.Dispose();
            this.webViewWindowThread = null;
        }

        protected override void AuthHttpServer_AuthorizationFinished(object sender, AuthEventArgs e)
        {
            try
            {
                base.AuthHttpServer_AuthorizationFinished(sender, e);
            }
            catch (Exception ex)
            {
                logger.Error($"Unhandled error in {nameof(this.AuthHttpServer_AuthorizationFinished)}", ex);
            }
            finally
            {
                this.webViewWindowThread?.Dispose();
                this.webViewWindowThread = null;
            }
        }
    }
}