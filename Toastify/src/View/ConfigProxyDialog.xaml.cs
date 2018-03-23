using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.ViewModel;

namespace Toastify.View
{
    public partial class ConfigProxyDialog : Window
    {
        private readonly ConfigProxyDialogViewModel viewModel;

        public bool AllowClosing { get; set; }

        public ConfigProxyDialog()
        {
            this.InitializeComponent();

            this.viewModel = new ConfigProxyDialogViewModel();
            this.DataContext = this.viewModel;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            hwndSource?.AddHook(this.HwndSourceHook);
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var wm = unchecked((Win32API.WindowsMessagesFlags)(uint)msg);

            switch (wm)
            {
                // When the window is shown, disable the system CLOSE button
                case Win32API.WindowsMessagesFlags.WM_SHOWWINDOW:
                    IntPtr hMenu = Win32API.GetSystemMenu(hwnd, false);
                    if (hMenu != IntPtr.Zero)
                        Win32API.EnableMenuItem(hMenu, Win32API.SysCommands.SC_CLOSE, Win32API.MenuFlags.MF_BYCOMMAND | Win32API.MenuFlags.MF_GRAYED);
                    break;

                // Intercept any WM_CLOSE event and cancel it if the window is not meant to be clos
                case Win32API.WindowsMessagesFlags.WM_CLOSE:
                    if (!this.AllowClosing)
                        handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            App.ProxyConfig = null;

            // Update the binding context
            this.DataContext = null;
            this.DataContext = this.viewModel;

            // Clear the password
            this.PasswordBox.Password = string.Empty;
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            this.AllowClosing = true;
            this.Close();
        }

        private void ConfigProxyDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            Settings.Current.UseProxy = true;
        }

        private void ConfigProxyDialog_OnClosed(object sender, EventArgs e)
        {
            string pwd = this.PasswordBox.Password;
            Security.SaveProtectedProxyPassword(Encoding.UTF8.GetBytes(pwd));
            App.ProxyConfig.Password = string.IsNullOrWhiteSpace(pwd) ? null : pwd;

            if (App.ProxyConfig.IsValid())
                Settings.Current.UseProxy = true;
            else
            {
                Settings.Current.UseProxy = false;
                App.ProxyConfig = null;
            }

            Settings.Current.ProxyConfig.Set(App.ProxyConfig);
            Settings.Current.Save();
        }

        private void TextBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
                textBox.SelectAll();
        }
    }
}