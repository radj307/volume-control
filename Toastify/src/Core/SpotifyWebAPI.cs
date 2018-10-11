using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public ICurrentlyPlayingObject GetCurrentlyPlayingTrack()
        {
            if (this.SpotifyWebApi == null || !this.WaitForTokenRefresh())
                return null;

            PlaybackContext playbackContext = this.SpotifyWebApi.GetPlayingTrack();
            LogReturnedValueIfError("Couldn't get the current playback context.", playbackContext);

            return playbackContext != null ? new CurrentlyPlayingObject(playbackContext) : null;
        }

        public ISpotifyUserProfile GetUserPrivateProfile()
        {
            if (this.SpotifyWebApi == null || !this.WaitForTokenRefresh())
                return null;

            PrivateProfile profile = this.SpotifyWebApi.GetPrivateProfile();
            LogReturnedValueIfError("Couldn't get the current user's private profile.", profile);

            return profile != null ? new SpotifyUserProfile(profile) : null;
        }

        public async Task<ICurrentlyPlayingObject> GetCurrentlyPlayingTrackAsync()
        {
            if (this.SpotifyWebApi == null || !this.WaitForTokenRefresh())
                return null;

            PlaybackContext playbackContext = await this.SpotifyWebApi.GetPlayingTrackAsync();
            LogReturnedValueIfError("Couldn't get the current playback context.", playbackContext);

            return playbackContext != null ? new CurrentlyPlayingObject(playbackContext) : null;
        }

        public async Task<ISpotifyUserProfile> GetUserPrivateProfileAsync()
        {
            if (this.SpotifyWebApi == null || !this.WaitForTokenRefresh())
                return null;

            PrivateProfile profile = await this.SpotifyWebApi.GetPrivateProfileAsync();
            LogReturnedValueIfError("Couldn't get the current user's private profile.", profile);

            return profile != null ? new SpotifyUserProfile(profile) : null;
        }

        private bool WaitForTokenRefresh()
        {
            if (!this.TokenManager.RefreshingTokenEvent.WaitOne(TimeSpan.FromSeconds(20)))
            {
                // TODO: Handle token refresh timeout
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
                          .Append($"   }}{Environment.NewLine}");
                    }

                    sb.Append("}");
                }
                else
                    sb.Append("null");

                logger.Warn(sb.ToString());
            }
        }

        #endregion
    }
}