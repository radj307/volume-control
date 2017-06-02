using System;

namespace Toastify.Plugin
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
        void Started();

        /// <summary>
        /// Called when Toastify is closing.
        /// </summary>
        void Closing();

        /// <summary>
        /// Called on track change.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        void TrackChanged(string artist, string title);
    }
}