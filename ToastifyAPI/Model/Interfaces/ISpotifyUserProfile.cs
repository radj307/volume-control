using ToastifyAPI.Core;

namespace ToastifyAPI.Model.Interfaces
{
    public interface ISpotifyUserProfile
    {
        SpotifySubscriptionLevel SubscriptionLevel { get; }
    }
}