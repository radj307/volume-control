using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;
using log4net;
using Toastify.Common;
using Toastify.Core;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using Toastify.ViewModel;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.RawInputAPI.Enums;
using ToastifyAPI.Native.RawInputAPI.Structs;
using Xceed.Wpf.Toolkit;
using MouseAction = ToastifyAPI.Core.MouseAction;
using RawInputApi = ToastifyAPI.Native.RawInputAPI.RawInput;
using WindowStartupLocation = System.Windows.WindowStartupLocation;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
namespace Toastify.View
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class SettingsView : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SettingsView));

        #region Static Fields and Properties

        private static SettingsView _current;

        #endregion

        private readonly ToastView toastView;
        private readonly SettingsViewModel settingsViewModel;

        #region Non-Public Properties

        private Settings Settings
        {
            get { return this.settingsViewModel?.Settings; }
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Events

        public static event EventHandler<SettingsViewLaunchedEventArgs> SettingsLaunched;

        public static event EventHandler SettingsClosed;

        public static event EventHandler<SettingsSavedEventArgs> SettingsSaved;

        #endregion

        private SettingsView(ToastView toastView)
        {
            Analytics.TrackEvent(Analytics.ToastifyEventCategory.General, Analytics.ToastifyEvent.SettingsLaunched);

            this.settingsViewModel = new SettingsViewModel();
            this.settingsViewModel.SettingsSaved += this.SettingsViewModel_SettingsSaved;
            this.settingsViewModel.HotkeyValidityChanged += this.SettingsViewModel_HotkeyValidityChanged;

            this.toastView = toastView;

            this.InitializeComponent();

            this.DataContext = this.settingsViewModel;
            this.WindowStartupLocation = this.StartupLocation;

            if (_current == null)
                _current = this;
        }

        private void UpdateHotkeyActivator(GenericHotkeyProxy hotkeyProxy, HotkeyType newHotkeyType, object newActivator, string text)
        {
            this.TxtSingleKey.Text = text;

            // This should be called before changing the hotkey type; otherwise, it will return the Key/MouseAction
            // used by the previous *keyboard/mouse hotkey* instead of the activator used by the previous *generic hotkey*.
            // This doesn't make any difference if we are not changing hotkey type.
            object previousActivator = hotkeyProxy.GetActivator();
            HotkeyType previousType = hotkeyProxy.Type;

            hotkeyProxy.Type = newHotkeyType;
            hotkeyProxy.SetActivator(newActivator);

            if (!previousActivator.Equals(hotkeyProxy.GetActivator()))
            {
                // Check every hotkey's validity again in case one of them has been set to use
                // the same activator this hotkey was previously using.
                this.VerifyHotkeysValidityAgainst(hotkeyProxy, null, previousType, previousActivator);
            }
        }

        private void VerifyHotkeysValidityAgainst(GenericHotkeyProxy hotkeyProxy, ModifierKeys? previousModifiers, HotkeyType? previousType, object previousActivator)
        {
            var hotkeys = this.settingsViewModel.Hotkeys;
            foreach (var hp in hotkeys)
            {
                if (hp == hotkeyProxy)
                    continue;

                if (hp.IsAlreadyInUseBy(previousModifiers ?? hotkeyProxy.Hotkey.Modifiers, previousType ?? hotkeyProxy.Type, previousActivator ?? hotkeyProxy.GetActivator()))
                {
                    if (!this.settingsViewModel.CheckIfHotkeyIsAlreadyInUse(hp))
                        hp.Hotkey.SetIsValid(true, null);
                }
            }
        }

        private void SetRawMouseInputHook(bool enable)
        {
            try
            {
                IntPtr hWnd = this.GetHandle(true);
                if (hWnd == IntPtr.Zero)
                {
                    // The app is probably terminating
                    return;
                }

                HwndSource source = HwndSource.FromHwnd(hWnd);
                if (source == null)
                {
                    logger.Error($"Raw mouse input hook not registered: couldn't get {nameof(HwndSource)}!");
                    return;
                }

                if (enable)
                {
                    if (RawInputApi.RegisterRawMouseInput(source.Handle))
                        source.AddHook(this.RawInputWndProc);
                    else
                        logger.Error($"Failed to register raw input mouse device. Error code: {Marshal.GetLastWin32Error()}");
                }
                else
                    source.RemoveHook(this.RawInputWndProc);
            }
            catch (Exception e)
            {
                logger.Error($"Unhandled error while {(enable ? "en" : "dis")}abling raw mouse input hook", e);
            }
        }

        private IntPtr RawInputWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                WindowsMessagesFlags wm = (WindowsMessagesFlags)msg;
                if (wm == WindowsMessagesFlags.WM_INPUT)
                {
                    int size = Marshal.SizeOf(typeof(RawInput));
                    if (User32.GetRawInputData(lParam, RawInputCommand.Input, out RawInput raw, ref size, Marshal.SizeOf(typeof(RawInputHeader))) != -1)
                    {
                        var mb = raw.Data.Mouse.ButtonFlags;
                        bool validRawMouseInputReceived = mb == RawMouseButtons.XButton1Up || mb == RawMouseButtons.XButton2Up || mb == RawMouseButtons.MouseWheel;
                        if (validRawMouseInputReceived && this.LstHotKeys.SelectedItem is GenericHotkeyProxy hotkeyProxy &&
                            this.TxtSingleKey.IsFocused && Processes.IsCurrentProcessFocused())
                        {
                            bool validButton = true;
                            MouseAction mouseAction = 0;

                            if (mb == RawMouseButtons.XButton1Up)
                                mouseAction = MouseAction.XButton1;
                            else if (mb == RawMouseButtons.XButton2Up)
                                mouseAction = MouseAction.XButton2;
                            else if (mb == RawMouseButtons.MouseWheel)
                            {
                                short delta = unchecked((short)raw.Data.Mouse.ButtonData);
                                if (delta > 0) // MWheelUp
                                    mouseAction = MouseAction.MWheelUp;
                                else if (delta < 0) // MWheelDown
                                    mouseAction = MouseAction.MWheelDown;
                            }
                            else
                                validButton = false;

                            if (validButton && Enum.IsDefined(typeof(MouseAction), mouseAction))
                                this.UpdateHotkeyActivator(hotkeyProxy, HotkeyType.MouseHook, mouseAction, mouseAction.ToString());
                        }
                    }
                }

                return IntPtr.Zero;
            }
            catch (Exception e)
            {
                logger.Error("Error", e);
                return IntPtr.Zero;
            }
        }

        #region Static Members

        public static void Launch(ToastView toastView)
        {
            if (_current != null)
            {
                _current.Activate();
                _current.SetRawMouseInputHook(true);
            }
            else
            {
                var settingsView = new SettingsView(toastView);
                SettingsLaunched?.Invoke(_current, new SettingsViewLaunchedEventArgs(settingsView.Settings, settingsView.settingsViewModel));
                settingsView.SetRawMouseInputHook(true);
                settingsView.ShowDialog();
            }
        }

        #endregion

        #region Event handlers

        private void Window_OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Current.SettingsWindowLastLocation = new WindowPosition((int)this.Left, (int)this.Top);
            Settings.Current.Save();

            this.SetRawMouseInputHook(false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SettingsClosed?.Invoke(_current, EventArgs.Empty);

            if (ReferenceEquals(_current, this))
                _current = null;
        }

        private void SettingsViewModel_SettingsSaved(object sender, SettingsSavedEventArgs e)
        {
            this.toastView.InitToast();
            this.toastView.DisplayAction(ToastifyActionEnum.SettingsSaved);

            this.LstHotKeys.GetBindingExpression(ItemsControl.ItemsSourceProperty)?.UpdateTarget();
            this.HotkeyGrid.GetBindingExpression(DataContextProperty)?.UpdateTarget();

            SettingsSaved?.Invoke(sender, e);
        }

        private void SettingsViewModel_HotkeyValidityChanged(object sender, EventArgs e)
        {
            this.LstHotKeys.Items.Refresh();
            this.HotkeyValidityGrid.GetBindingExpression(VisibilityProperty)?.UpdateTarget();
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
            List<ToastifyVolumeControlMode> currentItems = e.AddedItems.Cast<EnumComboBoxItem>()
                                                            .Select(it => (ToastifyVolumeControlMode)it.Value).ToList();
            if (!currentItems.Any())
                return;

            ToastifyVolumeControlMode current = currentItems.First();
            this.PanelWindowsMixerIncrement.IsEnabled = current == ToastifyVolumeControlMode.SystemSpotifyOnly;
        }

        #endregion "General" tab

        #region "Hotkeys" tab

        private void LstHotKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.LstHotKeys.SelectedItem is GenericHotkeyProxy hotkeyProxy)
            {
                Hotkey hotkey = hotkeyProxy.Hotkey;
                string text = string.Empty;

                if (hotkey is KeyboardHotkey kbdHotkey)
                    text = kbdHotkey.Key?.ToString();
                else if (hotkey is MouseHookHotkey mhHotkey)
                    text = mhHotkey.MouseButton?.ToString();

                this.TxtSingleKey.Text = text ?? string.Empty;
            }
        }

        private void TxtSingleKey_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.LstHotKeys.SelectedItem is GenericHotkeyProxy hotkeyProxy)
            {
                Key key = e.Key;
                if (key == Key.System)
                    key = e.SystemKey;

                if (key == Key.Tab)
                {
                    e.Handled = false;
                    return;
                }

                this.UpdateHotkeyActivator(hotkeyProxy, HotkeyType.Keyboard, key, key.ToString());
                e.Handled = true;
            }
        }

        #region modifiers

        private void CtrlToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnChecked(ModifierKeys.Control);
        }

        private void CtrlToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnUnchecked(ModifierKeys.Control);
        }

        private void AltToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnChecked(ModifierKeys.Alt);
        }

        private void AltToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnUnchecked(ModifierKeys.Alt);
        }

        private void ShiftToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnChecked(ModifierKeys.Shift);
        }

        private void ShiftToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnUnchecked(ModifierKeys.Shift);
        }

        private void WinToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnChecked(ModifierKeys.Windows);
        }

        private void WinToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.ModifierToggleButton_OnUnchecked(ModifierKeys.Windows);
        }

        private void ModifierToggleButton_OnChecked(ModifierKeys modifier)
        {
            if (this.LstHotKeys.SelectedItem is GenericHotkeyProxy hotkeyProxy)
                this.VerifyHotkeysValidityAgainst(hotkeyProxy, hotkeyProxy.Hotkey.Modifiers & ~modifier, null, null);
        }

        private void ModifierToggleButton_OnUnchecked(ModifierKeys modifier)
        {
            if (this.LstHotKeys.SelectedItem is GenericHotkeyProxy hotkeyProxy)
                this.VerifyHotkeysValidityAgainst(hotkeyProxy, hotkeyProxy.Hotkey.Modifiers | modifier, null, null);
        }

        #endregion

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
                Rect toastRect = ToastView.Current == null ? new Rect() : this.toastView.Rect;
                Rect totalRect = ScreenHelper.GetTotalWorkingArea();
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
                Rect toastRect = ToastView.Current == null ? new Rect() : this.toastView.Rect;
                Rect totalRect = ScreenHelper.GetTotalWorkingArea();
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

        #region "Advanced" tab

        private void ProxyPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.settingsViewModel != null)
                this.settingsViewModel.ProxyPassword = ((WatermarkPasswordBox)sender).SecurePassword;
        }

        private void ProxyTextBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
                textBox.SelectAll();
        }

        private void ToastifyBroadcasterWiki_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true };
            Process.Start(psi);
            e.Handled = true;
        }

        #endregion "Advanced" tab

        #endregion Event handlers

        #region ComboBox initialization for AutomationUI (accessibility)

        private static readonly object automationRootElementLock = new object();
        private AutomationElement mainAutomationWindow;

        private async void ComboBox_OnInitialized(object sender, EventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            if (sender is ComboBox cb)
                await Task.Run(() => this.InitializeExpandableElementForUIAutomation(cb)).ConfigureAwait(false);
        }

        private void InitializeExpandableElementForUIAutomation(IFrameworkInputElement element)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                AutomationElement automationElement;
                lock (automationRootElementLock)
                {
                    if (this.mainAutomationWindow == null)
                        this.mainAutomationWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, this.Name));

                    automationElement = this.mainAutomationWindow?.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, element.Name));
                }

                if (automationElement != null)
                {
                    AutomationPattern automationPatternFromElement = AutomationHelper.GetSpecifiedPattern(automationElement, "ExpandCollapsePatternIdentifiers.Pattern");
                    if (automationElement.GetCurrentPattern(automationPatternFromElement) is ExpandCollapsePattern expandCollapsePattern)
                    {
                        expandCollapsePattern.Expand();
                        expandCollapsePattern.Collapse();

                        this.General.Focus();
                    }
                }
            }));
        }

        #endregion
    }
}