using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.ViewModel;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;

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
            var wm = unchecked((WindowsMessagesFlags)(uint)msg);

            switch (wm)
            {
                // When the window is shown, disable the system CLOSE button
                case WindowsMessagesFlags.WM_SHOWWINDOW:
                    IntPtr hMenu = User32.GetSystemMenu(hwnd, false);
                    if (hMenu != IntPtr.Zero)
                        User32.EnableMenuItem(hMenu, SysCommands.SC_CLOSE, MenuFlags.MF_BYCOMMAND | MenuFlags.MF_GRAYED);
                    break;

                // Intercept any WM_CLOSE event and cancel it if the window is not meant to be clos
                case WindowsMessagesFlags.WM_CLOSE:
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
            if (string.IsNullOrEmpty(App.ProxyConfig.Username))
            {
                // If no username has been entered, remove the saved password
                Security.SaveProxyPassword(new SecureString());
            }
            else if (this.PasswordBox.SecurePassword.Length > 0)
            {
                // Otherwise, if the password box is not empty, save the new password
                Security.SaveProxyPassword(this.PasswordBox.SecurePassword);
            }
            else
            {
                // If the username field is not empty and the password's is,
                // then keep the previous password.
            }

            if (App.ProxyConfig.IsValid())
            {
                Settings.Current.UseProxy = true;
                App.ProxyConfig.Password = Security.GetSecureProxyPassword()?.ToPlainString();
            }
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