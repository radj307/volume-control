using log4net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Toastify.Common;
using Toastify.Core;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using Toastify.ViewModel;
using ToastifyAPI;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Delegates;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;
using Xceed.Wpf.Toolkit;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseAction = Toastify.Core.MouseAction;
using WindowStartupLocation = System.Windows.WindowStartupLocation;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
namespace Toastify.View
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class SettingsView : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SettingsView));

        private static SettingsView _current;

        private readonly ToastView toastView;
        private readonly SettingsViewModel settingsViewModel;

        private IntPtr hHook = IntPtr.Zero;
        private LowLevelMouseHookProc mouseHookProc;

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

        public static event EventHandler<SettingsSavedEventArgs> SettingsSaved;

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

        private void SetMouseHook(bool enable)
        {
            if (enable && this.hHook == IntPtr.Zero)
            {
                this.mouseHookProc = this.MouseHookProc;
                this.hHook = Processes.SetLowLevelMouseHook(ref this.mouseHookProc);
            }
            else if (!enable && this.hHook != IntPtr.Zero)
            {
                bool success = User32.UnhookWindowsHookEx(this.hHook);
                if (success == false)
                    logger.Error($"Failed to un-register a low-level mouse hook. Error code: {Marshal.GetLastWin32Error()}");

                this.hHook = IntPtr.Zero;
                this.mouseHookProc = null;
            }
        }

        private IntPtr MouseHookProc(int nCode, WindowsMessagesFlags wParam, LowLevelMouseHookStruct lParam)
        {
            if (nCode >= 0)
            {
                if (this.TxtSingleKey.IsFocused)
                {
                    if (wParam == WindowsMessagesFlags.WM_XBUTTONUP)
                    {
                        Union32 union = new Union32(lParam.mouseData);

                        if (union.High == 0x0001)
                        {
                            this.TxtSingleKey.Text = MouseAction.XButton1.ToString();
                            if (this.LstHotKeys.SelectedItem is Hotkey hotkey)
                                hotkey.KeyOrButton = MouseAction.XButton1;
                        }
                        else if (union.High == 0x0002)
                        {
                            this.TxtSingleKey.Text = MouseAction.XButton2.ToString();
                            if (this.LstHotKeys.SelectedItem is Hotkey hotkey)
                                hotkey.KeyOrButton = MouseAction.XButton2;
                        }
                    }
                    else if (wParam == WindowsMessagesFlags.WM_MOUSEWHEEL)
                    {
                        Union32 union = new Union32(lParam.mouseData);
                        
                        short delta = unchecked((short)union.High);
                        if (delta > 0)
                        {
                            // MWheelUp
                            this.TxtSingleKey.Text = MouseAction.MWheelUp.ToString();
                            if (this.LstHotKeys.SelectedItem is Hotkey hotkey)
                                hotkey.KeyOrButton = MouseAction.MWheelUp;
                        }
                        else if (delta < 0)
                        {
                            // MWheelDown
                            this.TxtSingleKey.Text = MouseAction.MWheelDown.ToString();
                            if (this.LstHotKeys.SelectedItem is Hotkey hotkey)
                                hotkey.KeyOrButton = MouseAction.MWheelDown;
                        }
                    }
                }
            }

            return User32.CallNextHookEx(this.hHook, nCode, wParam, lParam);
        }

        public static void Launch(ToastView toastView)
        {
            if (_current != null)
            {
                _current.Activate();
                _current.SetMouseHook(true);
            }
            else
            {
                SettingsView settingsView = new SettingsView(toastView);
                SettingsLaunched?.Invoke(_current, new SettingsViewLaunchedEventArgs(settingsView.Settings));
                settingsView.SetMouseHook(true);
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

            this.SetMouseHook(false);
        }

        private void SettingsViewModel_SettingsSaved(object sender, SettingsSavedEventArgs e)
        {
            this.toastView.InitToast();
            this.toastView.DisplayAction(ToastifyAction.SettingsSaved);
            SettingsSaved?.Invoke(sender, e);
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
                this.TxtSingleKey.Text = hotkey.KeyOrButton?.ToString() ?? string.Empty;
        }

        private void TxtSingleKey_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.Key;
            if (key == Key.System)
                key = e.SystemKey;

            this.TxtSingleKey.Text = key.ToString();

            if (this.LstHotKeys.SelectedItem is Hotkey hotkey)
                hotkey.KeyOrButton = key;
        }

        #endregion "Hotkeys" tab

        #region "Toast" tab

        private void FadeOutTimeUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.DisplayTimeUpDown.GetBindingExpression(DoubleUpDown.MaximumProperty)?.UpdateTarget();
        }

        private void FadeOutTimeUpDown_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.DisplayTimeUpDown.Increment = 1000;
        }

        private void FadeOutTimeUpDown_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.DisplayTimeUpDown.Increment = 100;
        }

        private void BorderThickness_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.Settings.ToastBorderThickness++;
            else if (this.Settings.ToastBorderThickness >= 1)
                this.Settings.ToastBorderThickness--;
        }

        #region Corner radius

        private void BorderTopLeftUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Settings.ToastBorderCornerRadiusTopLeft += (e.Delta > 0 ? 1 : -1) * (this.BorderTopLeftUpDown.Increment ?? 0.1);
        }

        private void BorderTopRightUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Settings.ToastBorderCornerRadiusTopRight += (e.Delta > 0 ? 1 : -1) * (this.BorderTopRightUpDown.Increment ?? 0.1);
        }

        private void BorderBottomLeftUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Settings.ToastBorderCornerRadiusBottomLeft += (e.Delta > 0 ? 1 : -1) * (this.BorderBottomLeftUpDown.Increment ?? 0.1);
        }

        private void BorderBottomRightUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Settings.ToastBorderCornerRadiusBottomRight += (e.Delta > 0 ? 1 : -1) * (this.BorderBottomRightUpDown.Increment ?? 0.1);
        }

        #endregion Corner radius

        #region Size & Position

        // MouseWheel

        private void ToastWidthUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool reachedMaxWidth = Math.Abs(this.toastView.Width - this.settingsViewModel.ToastWidthMax) < 1.0;
            if (e.Delta > 0 && reachedMaxWidth)
            {
                double increment = this.ToastWidthUpDown.Increment ?? 0.0;

                // Move the toast leftwards
                System.Windows.Rect toastRect = ToastView.Current == null ? new System.Windows.Rect() : this.toastView.Rect;
                System.Windows.Rect totalRect = ScreenHelper.GetTotalWorkingArea();
                double availableSpaceToTheLeft = toastRect.Left - totalRect.Left;
                double deltaX = Math.Min(availableSpaceToTheLeft, increment);

                this.Settings.PositionLeft -= deltaX;
                this.Settings.ToastWidth += deltaX;
            }
        }

        private void ToastHeightUpDown_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool reachedMaxHeight = Math.Abs(this.toastView.Height - this.settingsViewModel.ToastHeightMax) < 1.0;
            if (e.Delta > 0 && reachedMaxHeight)
            {
                double increment = this.ToastHeightUpDown.Increment ?? 0.0;

                // Move the toast upwards
                System.Windows.Rect toastRect = ToastView.Current == null ? new System.Windows.Rect() : this.toastView.Rect;
                System.Windows.Rect totalRect = ScreenHelper.GetTotalWorkingArea();
                double availableSpaceAbove = toastRect.Top - totalRect.Top;
                double deltaY = Math.Min(availableSpaceAbove, increment);

                this.Settings.PositionTop -= deltaY;
                this.Settings.ToastHeight += deltaY;
            }
        }

        // OnValueChanged

        private void ToastWidthUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.PositionLeftUpDown.GetBindingExpression(DoubleUpDown.MaximumProperty)?.UpdateTarget();
        }

        private void ToastHeightUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.PositionTopUpDown.GetBindingExpression(DoubleUpDown.MaximumProperty)?.UpdateTarget();
        }

        private void PositionLeftUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.ToastWidthUpDown.GetBindingExpression(DoubleUpDown.MaximumProperty)?.UpdateTarget();
        }

        private void PositionTopUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.ToastHeightUpDown.GetBindingExpression(DoubleUpDown.MaximumProperty)?.UpdateTarget();
        }

        // KeyDown / KeyUp

        private void ToastWidthUpDown_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.ToastWidthUpDown.Increment = 10.0;
        }

        private void ToastHeightUpDown_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.ToastHeightUpDown.Increment = 10.0;
        }

        private void PositionLeftUpDown_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.PositionLeftUpDown.Increment = 10.0;
        }

        private void PositionTopUpDown_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.PositionTopUpDown.Increment = 10.0;
        }

        private void ToastWidthUpDown_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.ToastWidthUpDown.Increment = 1.0;
        }

        private void ToastHeightUpDown_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.ToastHeightUpDown.Increment = 1.0;
        }

        private void PositionLeftUpDown_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.PositionLeftUpDown.Increment = 1.0;
        }

        private void PositionTopUpDown_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                this.PositionTopUpDown.Increment = 1.0;
        }

        #endregion Size & Position

        #endregion "Toast" tab

        #endregion Event handlers
    }
}