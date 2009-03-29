using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toastify
{
    public class SkypePlugin  : Toastify.Plugin.PluginBase
    {
        SKYPE4COMLib.Profile skypeProfile = null;
        SKYPE4COMLib.SkypeClass skypeClass = new SKYPE4COMLib.SkypeClass();
        private string oldMessage = string.Empty;

        public SkypePlugin()
        {
            if (skypeClass.Client.IsRunning != true)
                skypeClass.Client.Start(true, true);

            skypeProfile = skypeClass.CurrentUserProfile;
            oldMessage = skypeProfile.MoodText;
        }

        public void Init(string settings)
        {
            //Nothing
        }
        
        public void Started()
        {
            //Nothing
        }

        public void Closing()
        {
            //Reset old skype text
            skypeProfile.MoodText = this.oldMessage;
        }

        public void TrackChanged(string artist, string title)
        {
            skypeProfile.MoodText = string.Format("{0} - {1}", artist, title);
        }

        #region IDisposable Members

        public void Dispose()
        {
            skypeClass = null;
            skypeProfile = null;
        }

        #endregion
    }
}
