using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using log4net;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using Toastify.View;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;

namespace Toastify.Core.Auth
{
    public class ToastifyWebAuth : BaseSpotifyWebAuth, IDisposable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyWebAuth));

        private AuthHttpServer authHttpServer;

        private WebView webViewWindow;
        private Thread webViewThread;

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
            this.CloseWebView();
            this.webViewThread = new Thread(this.WebViewThreadStart);
            this.webViewThread.SetApartmentState(ApartmentState.STA);
            this.webViewThread.IsBackground = true;
            this.webViewThread.Start();
        }

        public override Task<IToken> GetToken()
        {
            return this.authHttpServer == null ? null : base.GetToken();
        }

        protected override IToken CreateToken(SpotifyTokenResponse spotifyTokenResponse)
        {
            return new Token(spotifyTokenResponse);
        }

        private void CloseWebView()
        {
            Dispatcher.FromThread(this.webViewThread)?.Invoke(() =>
            {
                try
                {
                    if (this.webViewThread != null)
                    {
                        this.webViewWindow?.Hide();
                        this.webViewWindow?.Close();
                        this.webViewWindow = null;

                        this.webViewThread = null;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error($"Unhandled error while closing {nameof(ToastifyWebAuth)}'s WebView", ex);
                }
            });
        }

        private void WebViewThreadStart()
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                this.webViewWindow = new WebView
                {
                    Title = "Spotify Authorization",
                    AllowedHosts = new List<string> { "accounts.spotify.com", "localhost", string.Empty }
                };
                this.webViewWindow.SetSize(new Size(475, 512));
                this.webViewWindow.Closed += (s, e) => Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                this.webViewWindow.Show();
                AuthorizationCodeFlow.Authorize(url => this.webViewWindow.NavigateTo(url), this.Scopes, this.State, this.ShowDialog, "en");

                Dispatcher.Run();
            }
            catch (Exception ex)
            {
                logger.Error($"Unhandled error in {nameof(ToastifyWebAuth)}'s WebView thread", ex);
            }
        }

        public void Dispose()
        {
            this.authHttpServer?.Dispose();
            this.authHttpServer = null;

            this.CloseWebView();
        }

        protected override void AuthHttpServer_AuthorizationFinished(object sender, AuthEventArgs e)
        {
            try
            {
                base.AuthHttpServer_AuthorizationFinished(sender, e);

                Dispatcher.FromThread(this.webViewThread)?.BeginInvoke(new Action(() =>
                {
                    this.webViewWindow.NavigateToRawHtml("<!DOCTYPE html><html><head></head><body><div>You can now close this window</div></body></html>");
                    this.CloseWebView();
                }));
            }
            catch (Exception ex)
            {
                logger.Error($"Unhandled error in {nameof(this.AuthHttpServer_AuthorizationFinished)}", ex);
            }
        }
    }
}