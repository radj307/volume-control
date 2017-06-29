using System;
using ToastifyAPI.Events;

namespace ToastifyAPI.Plugins
{
    public interface IPluginBase : IDisposable
    {
        /// <summary>
        /// Called directly after the constructor.
        /// </summary>
        /// <param name="settings">Data from the Settings element in the xml.</param>
        void Init(string settings);

        /// <summary>
        /// Called when Toastify is first started. After Init().
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Started(object sender, EventArgs e);

        /// <summary>
        /// Called when Toastify is closing.
        /// </summary>
        void Closing(object sender, EventArgs e);

        /// <summary>
        /// Called on track change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TrackChanged(object sender, SpotifyTrackChangedEventArgs e);
    }
}