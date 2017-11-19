using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Toastify.Common;
using Toastify.Core;
using Toastify.Events;
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

        private Settings Settings
        {
            get { return this.settingsViewModel?.Settings; }
        }

        public WindowStartupLocation StartupLocation
        {
            get
            {
                WindowStartupLocation location;

                if (this.Settings.FirstRun || this.Settings.SettingsWindowLastLocation == WindowPosition.Zero)
                    location = WindowStartupLocation.CenterScreen;
                else
                {
                    this.Left = this.Settings.SettingsWindowLastLocation.Left;
                    this.Top = this.Settings.SettingsWindowLastLocation.Top;
                    location = WindowStartupLocation.Manual;
                }
                return location;
            }
        }

        public static event EventHandler<SettingsViewLaunchedEventArgs> SettingsLaunched;

        public static event EventHandler SettingsClosed;

        private SettingsView(ToastView toastView)
        {
            Analytics.TrackEvent(Analytics.ToastifyEventCategory.General, Analytics.ToastifyEvent.SettingsLaunched);

            this.settingsViewModel = new SettingsViewModel();
            this.settingsViewModel.SettingsSaved += this.SettingsViewModel_SettingsSaved;

            this.toastView = toastView;

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
                SettingsLaunched?.Invoke(_current, new SettingsViewLaunchedEventArgs(settingsView.Settings));
                settingsView.ShowDialog();
            }
        }

        #region Event handlers

        private void Window_Closed(object sender, EventArgs e)
        {
            SettingsClosed?.Invoke(_current, EventArgs.Empty);

            Settings.Current.SettingsWindowLastLocation = new WindowPosition((int)this.Left, (int)this.Top);
            Settings.Current.Save();

            if (ReferenceEquals(_current, this))
                _current = null;
        }

        private void SettingsViewModel_SettingsSaved(object sender, EventArgs e)
        {
            this.toastView.InitToast();
            this.toastView.DisplayAction(SpotifyAction.SettingsSaved);
        }

        private void BtnDefaultMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.settingsViewModel.DefaultCommand?.Execute();
            this.BtnDefault.IsOpen = false;
        }

        private void BtnDefaultAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.settingsViewModel.DefaultAllCommand?.Execute();
            this.BtnDefault.IsOpen = false;
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
                this.Settings.FadeOutTime += 10;
            else if (this.Settings.FadeOutTime >= 10)
                this.Settings.FadeOutTime -= 10;
        }

        private void BorderThickness_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.ToastBorderThickness++;
            else if (this.Settings.ToastBorderThickness >= 1)
                this.Settings.ToastBorderThickness--;
        }

        #region Change colors

        private void TopColorAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string alpha = Convert.ToByte(e.NewValue).ToString("X2");
            this.Settings.ToastColorTop = $"#{alpha}{this.Settings.ToastColorTop.Substring(3)}";
        }

        private void BottomColorAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string alpha = Convert.ToByte(e.NewValue).ToString("X2");
            this.Settings.ToastColorBottom = $"#{alpha}{this.Settings.ToastColorBottom.Substring(3)}";
        }

        private void BorderColorAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string alpha = Convert.ToByte(e.NewValue).ToString("X2");
            this.Settings.ToastBorderColor = $"#{alpha}{this.Settings.ToastBorderColor.Substring(3)}";
        }

        #endregion Change colors

        #region Corner radius

        private void CornerTopLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                this.Settings.ToastBorderCornerRadiusTopLeft += 0.1;
            }
            else if (this.Settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                this.Settings.ToastBorderCornerRadiusTopLeft -= 0.1;
        }

        private void CornerTopRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.ToastBorderCornerRadiusTopRight += 0.1;
            else if (this.Settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                this.Settings.ToastBorderCornerRadiusTopRight -= 0.1;
        }

        private void CornerBottomLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.ToastBorderCornerRadiusBottomLeft += 0.1;
            else if (this.Settings.ToastBorderCornerRadiusBottomLeft >= 0.1)
                this.Settings.ToastBorderCornerRadiusBottomLeft -= 0.1;
        }

        private void CornerBottomRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.ToastBorderCornerRadiusBottomRight += 0.1;
            else if (this.Settings.ToastBorderCornerRadiusBottomRight >= 0.1)
                this.Settings.ToastBorderCornerRadiusBottomRight -= 0.1;
        }

        #endregion Corner radius

        #region Toast size & position

        private void ToastWidth_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.ToastWidth += 5;
            else if (this.Settings.ToastWidth >= 205)
                this.Settings.ToastWidth -= 5;
        }

        private void ToastHeight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.ToastHeight += 5;
            else if (this.Settings.ToastHeight >= 70)
                this.Settings.ToastHeight -= 5;
        }

        private void PositionLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.PositionLeft++;
            else if (this.Settings.PositionLeft > 0)
                this.Settings.PositionLeft--;
        }

        private void PositionTop_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.PositionTop++;
            else if (this.Settings.PositionTop > 0)
                this.Settings.PositionTop--;
        }

        #endregion Toast size & position

        #endregion "Toast" tab

        #endregion Event handlers
    }
}