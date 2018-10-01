using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;

namespace ToastifyAPI.Core.Auth.ToastifyWebAuthAPI
{
    public abstract class BaseSpotifyWebAuth : ISpotifyWebAuth
    {
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
            await this.AuthHttpServer.Start();

            AuthorizationCodeFlow.Authorize(this.Scopes, this.State, this.ShowDialog);
            while (this.authResponse == null)
            {
                await Task.Delay(100);
            }

            if (this.authResponse.Error == null && !string.IsNullOrWhiteSpace(this.authResponse.Code))
            {
                if (this.authResponse.State == this.State)
                {
                    HttpResponse httpResponse = new HttpResponse(256, 1024);
                    SpotifyTokenResponse spotifyTokenResponse = new SpotifyTokenResponse(256, 32, 736);
                    AuthorizationCodeFlow.GetAuthorizationToken(ref httpResponse, ref spotifyTokenResponse, this.authResponse.Code);

                    if (httpResponse.status == (int)HttpStatusCode.OK)
                    {
                        spotifyTokenResponse.CreationDate = DateTime.Now;
                        this.token = this.CreateToken(spotifyTokenResponse);
                        this.tokenReturned = true;
                        return this.token;
                    }
                }
            }

            this.tokenReturned = true;
            return null;
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
                refreshedToken = this.CreateToken(spotifyTokenResponse);
            }

            return Task.FromResult(refreshedToken);
        }

        protected abstract IToken CreateToken(SpotifyTokenResponse spotifyTokenResponse);

        private async void AuthHttpServer_AuthorizationFinished(object sender, AuthEventArgs e)
        {
            this.AuthHttpServer.AuthorizationFinished -= this.AuthHttpServer_AuthorizationFinished;
            await this.AuthHttpServer.Stop();

            this.authResponse = e;
        }
    }
}