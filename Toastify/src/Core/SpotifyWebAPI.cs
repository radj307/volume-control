using SpotifyAPI.Web.Models;
using Toastify.Model;
using ToastifyAPI.Core;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Model.Interfaces;
using SpotifyAPIWebAPI = SpotifyAPI.Web.SpotifyWebAPI;

namespace Toastify.Core
{
    public class SpotifyWebAPI : ISpotifyWebAPI
    {
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

        public ICurrentlyPlayingObject GetCurrentlyPlayingTrack()
        {
            PlaybackContext playbackContext = this.SpotifyWebApi?.GetPlayingTrack();
            return playbackContext != null ? new CurrentlyPlayingObject(playbackContext) : null;
        }

        public ISpotifyUserProfile GetUserPrivateProfile()
        {
            PrivateProfile profile = this.SpotifyWebApi?.GetPrivateProfile();
            return profile != null ? new SpotifyUserProfile(profile) : null;
        }
    }
}