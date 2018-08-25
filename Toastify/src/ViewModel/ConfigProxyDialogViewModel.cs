using System;
using System.Windows.Media;
using Toastify.Common;
using Toastify.Helpers;
using ToastifyAPI.Native.Enums;

namespace Toastify.ViewModel
{
    public class ConfigProxyDialogViewModel : ObservableObject
    {
        #region Public Properties

        public string Host
        {
            get { return App.ProxyConfig.Host; }
            set
            {
                if (App.ProxyConfig.Host != value)
                {
                    App.ProxyConfig.Host = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int Port
        {
            get { return App.ProxyConfig.Port; }
            set
            {
                if (App.ProxyConfig.Port != value)
                {
                    // ReSharper disable once BuiltInTypeReferenceStyle
                    App.ProxyConfig.Port = value != 0 ? value.Clamp(1, UInt16.MaxValue) : 80;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public string Username
        {
            get { return App.ProxyConfig.Username; }
            set
            {
                if (App.ProxyConfig.Username != value)
                {
                    App.ProxyConfig.Username = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool BypassProxyOnLocal
        {
            get { return App.ProxyConfig.BypassProxyOnLocal; }
            set
            {
                if (App.ProxyConfig.BypassProxyOnLocal != value)
                {
                    App.ProxyConfig.BypassProxyOnLocal = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ImageSource InfoIcon
        {
            get { return ToastifyAPI.Win32API.GetStockIconImage(ShStockIconId.SIID_INFO, true); }
        }

        #endregion
    }
}