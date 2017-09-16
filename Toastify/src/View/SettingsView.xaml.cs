using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Toastify.Common;
using Toastify.Core;
using Toastify.Model;
using Toastify.Services;
using Toastify.ViewModel;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
namespace Toastify.View
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class SettingsView : Window
    {
        private static SettingsView _current;

        private readonly ToastView toastView;
        private readonly SettingsViewModel settingsViewModel;
        private readonly Settings settings;

        public WindowStartupLocation StartupLocation
        {
            get
            {
                WindowStartupLocation location;

                if (this.settings.FirstRun || this.settings.SettingsWindowLastLocation == WindowPosition.Zero)
                    location = WindowStartupLocation.CenterScreen;
                else
                {
                    this.Left = this.settings.SettingsWindowLastLocation.Left;
                    this.Top = this.settings.SettingsWindowLastLocation.Top;
                    location = WindowStartupLocation.Manual;
                }
                return location;
            }
        }

        public static event EventHandler SettingsLaunched;

        public static event EventHandler SettingsClosed;

        private SettingsView(ToastView toastView)
        {
            Analytics.TrackEvent(Analytics.ToastifyEventCategory.General, Analytics.ToastifyEvent.SettingsLaunched);

            this.settingsViewModel = new SettingsViewModel();
            this.settingsViewModel.SettingsSaved += this.SettingsViewModel_SettingsSaved;

            this.toastView = toastView;
            this.settings = this.settingsViewModel.Settings;

            this.InitializeComponent();

            this.DataContext = this.settingsViewModel;
            this.WindowStartupLocation = this.StartupLocation;

            if (_current == null)
                _current = this;
        }

        public static void Launch(ToastView toastView)
        {
            if (_current != null)
                _current.Activate();
            else
            {
                SettingsView settingsView = new SettingsView(toastView);
                SettingsLaunched?.Invoke(_current, EventArgs.Empty);
                settingsView.ShowDialog();
            }
        }

        #region Event handlers

        private void Window_Closed(object sender, EventArgs e)
        {
            SettingsClosed?.Invoke(_current, EventArgs.Empty);

            Settings.Instance.SettingsWindowLastLocation = new WindowPosition((int)this.Left, (int)this.Top);
            Settings.Instance.Save(true);

            if (ReferenceEquals(_current, this))
                _current = null;
        }

        private void SettingsViewModel_SettingsSaved(object sender, EventArgs e)
        {
            this.toastView.InitToast();
            this.toastView.DisplayAction(SpotifyAction.SettingsSaved);
        }

        #region "General" tab

        private void ComboVolumeControlMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentItems = e.AddedItems.Cast<EnumComboBoxItem>()
                                           .Select(it => (ToastifyVolumeControlMode)it.Value).ToList();
            if (!currentItems.Any())
                return;

            var current = currentItems.First();
            this.PanelWindowsMixerIncrement.IsEnabled = current == ToastifyVolumeControlMode.SystemSpotifyOnly;
        }

        #endregion "General" tab

        #region "Hotkeys" tab

        private void LstHotKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.LstHotKeys.SelectedItem is Hotkey hotkey)
                this.TxtSingleKey.Text = hotkey.Key.ToString();
        }

        private void TxtSingleKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.Key;
            if (key == Key.System)
                key = e.SystemKey;

            this.TxtSingleKey.Text = key.ToString();

            if (this.LstHotKeys.SelectedItem is Hotkey hotkey)
                hotkey.Key = key;
        }

        #endregion "Hotkeys" tab

        #region "Toast" tab

        private void FadeOutTime_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.FadeOutTime += 10;
            else if (this.settings.FadeOutTime >= 10)
                this.settings.FadeOutTime -= 10;
        }

        private void BorderThickness_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderThickness++;
            else if (this.settings.ToastBorderThickness >= 1)
                this.settings.ToastBorderThickness--;
        }

        #region Change colors

        private void TopColorAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string alpha = Convert.ToByte(e.NewValue).ToString("X2");
            this.settings.ToastColorTop = $"#{alpha}{this.settings.ToastColorTop.Substring(3)}";
        }

        private void BottomColorAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string alpha = Convert.ToByte(e.NewValue).ToString("X2");
            this.settings.ToastColorBottom = $"#{alpha}{this.settings.ToastColorBottom.Substring(3)}";
        }

        private void BorderColorAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string alpha = Convert.ToByte(e.NewValue).ToString("X2");
            this.settings.ToastBorderColor = $"#{alpha}{this.settings.ToastBorderColor.Substring(3)}";
        }

        #endregion Change colors

        #region Corner radius

        private void CornerTopLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.settings.ToastBorderCornerRadiusTopLeft += 0.1;
            }
            else if (this.settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                this.settings.ToastBorderCornerRadiusTopLeft -= 0.1;
        }

        private void CornerTopRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderCornerRadiusTopRight += 0.1;
            else if (this.settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                this.settings.ToastBorderCornerRadiusTopRight -= 0.1;
        }

        private void CornerBottomLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderCornerRadiusBottomLeft += 0.1;
            else if (this.settings.ToastBorderCornerRadiusBottomLeft >= 0.1)
                this.settings.ToastBorderCornerRadiusBottomLeft -= 0.1;
        }

        private void CornerBottomRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastBorderCornerRadiusBottomRight += 0.1;
            else if (this.settings.ToastBorderCornerRadiusBottomRight >= 0.1)
                this.settings.ToastBorderCornerRadiusBottomRight -= 0.1;
        }

        #endregion Corner radius

        #region Toast size & position

        private void ToastWidth_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastWidth += 5;
            else if (this.settings.ToastWidth >= 205)
                this.settings.ToastWidth -= 5;
        }

        private void ToastHeight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.ToastHeight += 5;
            else if (this.settings.ToastHeight >= 70)
                this.settings.ToastHeight -= 5;
        }

        private void PositionLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.PositionLeft++;
            else if (this.settings.PositionLeft > 0)
                this.settings.PositionLeft--;
        }

        private void PositionTop_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.settings.PositionTop++;
            else if (this.settings.PositionTop > 0)
                this.settings.PositionTop--;
        }

        #endregion Toast size & position

        #endregion "Toast" tab

        #endregion Event handlers
    }
}