using SpotifyAPI.Web.Models;
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public class SpotifyUserProfile : ISpotifyUserProfile
    {
        #region Public Properties

        public SpotifySubscriptionLevel SubscriptionLevel { get; }

        #endregion

        public SpotifyUserProfile(PrivateProfile privateProfile)
        {
            switch (privateProfile.Product)
            {
                case "free":
                    this.SubscriptionLevel = SpotifySubscriptionLevel.Free;
                    break;

                case "open":
                    this.SubscriptionLevel = SpotifySubscriptionLevel.Open;
                    break;

                case "premium":
                    this.SubscriptionLevel = SpotifySubscriptionLevel.Premium;
                    break;

                default:
                    this.SubscriptionLevel = SpotifySubscriptionLevel.Unknown;
                    break;
            }
        }

        public SpotifyUserProfile(PublicProfile publicProfile)
        {
            this.SubscriptionLevel = SpotifySubscriptionLevel.Unknown;
        }
    }
}