using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toastify.Plugin
{
    public interface PluginBase : IDisposable
    {
        /// <summary>
        /// Is called directly after the constructor.
        /// </summary>
        /// <param name="settings">Data from the Settings element in the xml.</param>
        void Init(string settings);
        /// <summary>
        /// Is called when Toastify is first started. After Init().
        /// </summary>
        void Started();
        /// <summary>
        /// Is called when Toastify is closing.
        /// </summary>
        void Closing();
        /// <summary>
        /// Is called on track change.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        void TrackChanged(string artist, string title);
    }
}
