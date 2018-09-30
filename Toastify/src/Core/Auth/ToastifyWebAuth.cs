using System;
using System.Threading.Tasks;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;

namespace Toastify.Core.Auth
{
    public class ToastifyWebAuth : BaseSpotifyWebAuth, IDisposable
    {
        private AuthHttpServer authHttpServer;

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

        public ToastifyWebAuth(string scopes, string state, bool showDialog)
        {
            this.Scopes = scopes;
            this.State = state;
            this.ShowDialog = showDialog;

            this.authHttpServer = new AuthHttpServer();
        }

        public override Task<IToken> GetToken()
        {
            return this.authHttpServer == null ? null : base.GetToken();
        }

        protected override IToken CreateToken(SpotifyTokenResponse spotifyTokenResponse)
        {
            return new Token
            {
                AccessToken = spotifyTokenResponse.accessToken,
                TokenType = spotifyTokenResponse.tokenType,
                ExpiresIn = spotifyTokenResponse.expiresIn,
                RefreshToken = spotifyTokenResponse.refreshToken,
                CreateDate = spotifyTokenResponse.CreationDate
            };
        }

        public void Dispose()
        {
            this.authHttpServer?.Dispose();
            this.authHttpServer = null;
        }
    }
}