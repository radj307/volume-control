using System.Threading.Tasks;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Core
{
    public interface IToastifyBroadcaster
    {
        #region Public Properties

        uint Port { get; }

        #endregion

        Task StartAsync();
        Task StopAsync();

        Task BroadcastCurrentTrack<T>(T track) where T : ISpotifyTrack;
        Task BroadcastPlayState(bool playing);
    }
}