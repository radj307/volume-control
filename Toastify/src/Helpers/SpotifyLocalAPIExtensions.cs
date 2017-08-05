using SpotifyAPI.Local;
using Toastify.Core;
using Toastify.Model;

namespace Toastify.Helpers
{
    internal static class SpotifyLocalAPIExtensions
    {
        public static void IncrementVolume(this SpotifyLocalAPI api)
        {
            float currentVolume = api.GetSpotifyVolume();
            float newVolume = currentVolume + Settings.Instance.WindowsVolumeMixerIncrement;
            api.SetSpotifyVolume(newVolume >= 100.0f ? 100.0f : newVolume);
        }

        public static void DecrementVolume(this SpotifyLocalAPI api)
        {
            float currentVolume = api.GetSpotifyVolume();
            float newVolume = currentVolume - Settings.Instance.WindowsVolumeMixerIncrement;
            api.SetSpotifyVolume(newVolume <= 0.0f ? 0.0f : newVolume);
        }

        public static void ToggleMute(this SpotifyLocalAPI api)
        {
            if (api.IsSpotifyMuted())
                api.UnMute();
            else
                api.Mute();
        }
    }
}