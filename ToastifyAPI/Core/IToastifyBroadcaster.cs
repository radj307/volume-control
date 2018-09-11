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

        Task BroadcastCurrentSong<T>(T song) where T : ISong;
        Task BroadcastPlayState(bool playing);
    }
}