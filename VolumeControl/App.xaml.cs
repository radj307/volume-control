using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
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

        #region ApplicationEventHandlers
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.Info("Application is starting.");
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Save();
            Settings.Reload();
            Log.Info("Application is exiting.");
        }
        #endregion ApplicationEventHandlers
    }
}
