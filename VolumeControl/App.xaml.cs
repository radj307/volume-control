using System.Windows;
using VolumeControl.Log;
using VolumeControl.Properties;

namespace VolumeControl
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        #region Properties
        private static Settings Settings => Settings.Default;
        private static LogWriter Log => FLog.Log;
        #endregion Properties
    }
}
