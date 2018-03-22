using System;
using System.Windows;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.ViewModel;

namespace Toastify.View
{
    public partial class ConfigProxyDialog : Window
    {
        public ConfigProxyDialog()
        {
            this.InitializeComponent();
            this.DataContext = new ConfigProxyDialogViewModel();
        }

        private void ConfigProxyDialog_OnClosed(object sender, EventArgs e)
        {
            if (App.ProxyConfig.IsValid())
            {
                Settings.Current.UseProxy = true;
                Settings.Current.ProxyConfig.Set(App.ProxyConfig);
                Settings.Current.Save();
            }
        }
    }
}