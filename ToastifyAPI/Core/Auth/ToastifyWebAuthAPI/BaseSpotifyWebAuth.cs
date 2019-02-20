using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using log4net;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;

namespace ToastifyAPI.Core.Auth.ToastifyWebAuthAPI
{
    public abstract class BaseSpotifyWebAuth : ISpotifyWebAuth
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BaseSpotifyWebAuth));

        private IToken token;
        private AuthEventArgs authResponse;
        private bool tokenReturned;

        #region Non-Public Properties

        protected abstract IAuthHttpServer AuthHttpServer { get; }

        #endregion

        #region Public Properties

        public abstract string Scopes { get; }
        public abstract string State { get; }
        public abstract bool ShowDialog { get; }

        #endregion

        public virtual async Task<IToken> GetToken()
        {
            if (this.tokenReturned)
                return this.token;

            if (this.AuthHttpServer == null)
                return null;

            this.AuthHttpServer.AuthorizationFinished += this.AuthHttpServer_AuthorizationFinished;
            await this.AuthHttpServer.Start().ConfigureAwait(false);

            this.Authorize();
            bool shouldAbortAuthorization = false;
            while (this.authResponse == null && !shouldAbortAuthorization)
            {
                await Task.Delay(100).ConfigureAwait(false);
                shouldAbortAuthorization = await this.ShouldAbortAuthorization().ConfigureAwait(false);
            }

            if (this.authResponse == null && shouldAbortAuthorization)
            {
                this.AuthHttpServer.AuthorizationFinished -= this.AuthHttpServer_AuthorizationFinished;
                await this.AuthHttpServer.Stop().ConfigureAwait(false);
                return null;
            }

            if (this.authResponse != null && this.authResponse.Error == null && !string.IsNullOrWhiteSpace(this.authResponse.Code))
            {
                if (this.authResponse.State == this.State || string.IsNullOrWhiteSpace(this.authResponse.State) && string.IsNullOrWhiteSpace(this.State))
                {
                    HttpResponse httpResponse = new HttpResponse(256, 1024);
                    SpotifyTokenResponse spotifyTokenResponse = new SpotifyTokenResponse(256, 32, 736);
                    AuthorizationCodeFlow.GetAuthorizationToken(ref httpResponse, ref spotifyTokenResponse, this.authResponse.Code);

                    if (httpResponse.status == (int)HttpStatusCode.OK)
                    {
                        spotifyTokenResponse.CreationDate = DateTime.Now;
                        this.token = this.CreateToken(spotifyTokenResponse);
                        this.tokenReturned = true;
                    }
                    else
                    {
                        // TODO: Handle HTTP status != OK
                        logger.Debug("TODO: Handle HTTP status != OK");
                    }
                }
            }

            this.tokenReturned = true;
            return this.token;
        }

        public Task<IToken> RefreshToken([NotNull] IToken token)
        {
            IToken refreshedToken = null;

            HttpResponse httpResponse = new HttpResponse(256, 1024);
            SpotifyTokenResponse spotifyTokenResponse = new SpotifyTokenResponse(256, 32, 736);
            AuthorizationCodeFlow.RefreshAuthorizationToken(ref httpResponse, ref spotifyTokenResponse, token.RefreshToken);

            if (httpResponse.status == (int)HttpStatusCode.OK)
            {
                spotifyTokenResponse.CreationDate = DateTime.Now;
                spotifyTokenResponse.refreshToken = token.RefreshToken;   // The refresh API endpoint does not return the refresh token itself
                refreshedToken = this.CreateToken(spotifyTokenResponse);
            }
            else
            {
                // TODO: Handle HTTP status != OK
                logger.Debug("TODO: Handle HTTP status != OK");
            }

            return Task.FromResult(refreshedToken);
        }

        protected abstract void Authorize();

        protected abstract IToken CreateToken(SpotifyTokenResponse spotifyTokenResponse);

        protected abstract Task<bool> ShouldAbortAuthorization();

        protected virtual async void AuthHttpServer_AuthorizationFinished(object sender, AuthEventArgs e)
        {
            this.AuthHttpServer.AuthorizationFinished -= this.AuthHttpServer_AuthorizationFinished;
            await this.AuthHttpServer.Stop().ConfigureAwait(false);

            this.authResponse = e;
        }

        public abstract void Dispose();
    }
}