using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Extensions;
using VolumeControl.Core.Input.Enums;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.SDK.Internal;
using VolumeControl.TypeExtensions;
using VolumeControl.ViewModels;
using VolumeControl.WPF;
using VolumeControl.WPF.Extensions;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for DeviceListNotification.xaml
    /// </summary>
    public partial class DeviceListNotification : Window
    {
        #region Constructor
        public DeviceListNotification()
        {
            InitializeComponent();

            // setup the timeout timer
            if (Settings.DeviceListNotificationConfig.TimeoutMs <= 0)
            { // validate the timeout value before using it for the timer interval to prevent possible unhandled exception
                int defaultValue = NotificationConfigSection.DefaultTimeoutMs;
                FLog.Error($"{nameof(DeviceListNotification)} {nameof(Settings.DeviceListNotificationConfig.TimeoutMs)} cannot be less than or equal to zero; it was reset to '{defaultValue}' in order to avoid a fatal exception.",
                    new ArgumentOutOfRangeException($"{nameof(Settings)}.{Settings.DeviceListNotificationConfig}.{nameof(Settings.DeviceListNotificationConfig.TimeoutMs)}", Settings.DeviceListNotificationConfig.TimeoutMs, $"The value '{Settings.DeviceListNotificationConfig.TimeoutMs}' isn't valid for property 'System.Timers.Timer.Interval'; Value is out-of-range! (Minimum: 1)"));
                Settings.DeviceListNotificationConfig.TimeoutMs = defaultValue;
            }
            _timer = new()
            {
                AutoReset = false,
            };
            // bind the interval
            _timer.SetBinding(BindableTimer.IntervalProperty, new Binding()
            {
                Source = Settings,
                Path = new PropertyPath($"{nameof(Config.DeviceListNotificationConfig)}.{nameof(NotificationConfigSection.TimeoutMs)}"),
                Mode = BindingMode.OneWay
            });
            _timer.Elapsed += this._timer_Elapsed;

            // close the window when the main window closes
            Application.Current.MainWindow.Closed += (s, e) =>
            {
                _allowClose = true; //< must be true for this window to be able to close
                this.Close();
            };

            Settings.DeviceListNotificationConfig.PropertyChanged += this.DeviceListNotificationConfig_PropertyChanged;

            VCSettings.DeviceConfigVM.FlagsVM.StateChanged += this.FlagsVM_StateChanged;
            VCSettings.AudioAPI.AudioDeviceSelector.SelectedDeviceChanged += this.AudioDeviceSelector_SelectedDeviceChanged;

            // bind to the show event
            VCEvents.ShowDeviceListNotification += this.VCEvents_ShowDeviceListNotification;
        }
        #endregion Constructor

        #region Fields
        private readonly BindableTimer _timer;
        // true when the window has loaded
        private bool _loaded = false;
        /// <summary>
        /// <see langword="true"/> when the window is currently fading in OR fading out.
        /// </summary>
        /// <remarks>
        /// Don't use this directly, use the <see cref="IsFadingIn"/>/<see cref="IsFadingOut"/> property instead.
        /// </remarks>
        private bool _fading = false;
        /// <summary>
        /// <see langword="true"/> when the window is currently fading in.
        /// </summary>
        /// <remarks>
        /// Don't use this directly, use the <see cref="IsFadingIn"/> property instead.
        /// </remarks>
        private bool _fadingIn = false;
        // true when the window can close
        private bool _allowClose = false;
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        public VolumeControlVM VCSettings => (this.FindResource("Settings") as VolumeControlVM)!;
        public NotificationConfigSectionVM VM => VCSettings.DeviceConfigVM;
        private Storyboard FadeInStoryboard => (FindResource(nameof(FadeInStoryboard)) as Storyboard)!;
        private Storyboard FadeOutStoryboard => (FindResource(nameof(FadeOutStoryboard)) as Storyboard)!;
        private bool IsFadingIn
        {
            get => _fadingIn;
            set
            {
                _fading = value;
                _fadingIn = value;
            }
        }
        private bool IsFadingOut
        {
            get => _fading && !_fadingIn;
            set
            {
                _fading = value;
                _fadingIn = false;
            }
        }
        private bool IsHiddenByViewMode => VM.FlagsVM.State == ENotificationViewMode.Nothing || (VM.FlagsVM.State.HasAllFlags(ENotificationViewMode.ControlBar, ENotificationViewMode.SelectedItemOnly) && VCSettings.AudioAPI.SelectedDevice == null);
        #endregion Properties

        #region Methods

        #region Start/Stop-Timer
        /// <summary>
        /// Starts the timer if <see cref="Config.NotificationTimeoutEnabled"/> is <see langword="true"/>; otherwise does nothing.
        /// </summary>
        private void StartTimer()
        {
            if (!Settings.DeviceListNotificationConfig.TimeoutEnabled) return;
            _timer.Start();
        }
        /// <summary>
        /// Stops the <see cref="_timer"/>, preventing the <see cref="System.Timers.Timer.Elapsed"/> event from firing.
        /// </summary>
        private void StopTimer() => _timer.Stop();
        private void RestartTimer()
        {
            if (!Settings.DeviceListNotificationConfig.TimeoutEnabled) return;
            _timer.Restart();
        }
        #endregion Start/Stop-Timer

        #region Show/Hide
        public new void Show()
        {
            if (IsHiddenByViewMode) return;

            StopTimer();
            if (!Settings.DeviceListNotificationConfig.DoFadeIn)
            { // fade-in disabled:
                //< we are on the main UI thread here
                base.Show();
                StartTimer();
            }
            else if (IsVisible && (_fading && !_fadingIn))
            { // stop fade out animation and reset opacity instantly
                StopFadeOut();
                RestartTimer();
            }
            else if (!IsVisible)
            { // start fade in animation
                StartFadeIn();
            }
            else
            { // window is already visible and not fading out, restart the timer
                RestartTimer();
            }
        }
        public new void Hide()
        {
            if (!Settings.DeviceListNotificationConfig.DoFadeOut)
            { // fade-out disabled:
                //< we are NOT on the main UI thread here (!), use the dispatcher:
                HideNowNoFadeOut();
            }
            else if (!_fading)
            {
                StartFadeOut();
            }
            else if (_fadingIn)
            {
                StopFadeIn(resetOpacity: false);
                StartFadeOut();
            }
        }
        private void HideNowNoFadeOut()
        {
            if (_fadingIn)
                StopFadeIn(resetOpacity: false);
            this.Dispatcher.Invoke(() =>
            {
                StopTimer();
                base.Hide();
            });
            SavePosition();
        }
        #endregion Show/Hide

        #region Start/Stop Fade Animations
        /// <summary>
        /// Starts the <see cref="FadeInStoryboard"/> and makes the window appear. Does nothing if the window is already visible.
        /// </summary>
        private void StartFadeIn()
        {
            // if already visible OR already fading in, do nothing
            if (IsVisible || IsFadingIn) return;

            // window is not visible AND not fading in

            // make window fully transparent
            Opacity = 0.0;
            // make the window visible, don't start the timer yet
            //ForceShow(startTimer: false);
            base.Show();
            // fading animation is about to play, set fading in state to true
            IsFadingIn = true;
            // begin the storyboard animation
            this.Dispatcher.Invoke(() => FadeInStoryboard.Begin(this, isControllable: true));
        }
        /// <summary>
        /// Interrupts the <see cref="FadeInStoryboard"/>, and optionally resets the window opacity.
        /// </summary>
        /// <param name="resetOpacity">When <see langword="true"/>, the window opacity will be reset and the window will be fully opaque; othewise when <see langword="false"/>, the opacity is not changed.</param>
        private void StopFadeIn(bool resetOpacity = true)
        {
            // if not visible OR not fading in, do nothing
            if (!IsVisible || !IsFadingIn) return;

            // window is visible AND currently fading in

            // stop the storyboard animation
            this.Dispatcher.Invoke(() => FadeInStoryboard.Stop(this));
            // fading animation is not playing, set the fading in state to false
            IsFadingIn = false;
            // if resetOpacity is true, make the window fully opaque. otherwise, don't change the opacity
            if (resetOpacity)
            {
                Opacity = 1.0;
            }
            // start the timer, since the fade-in animation is complete
            if (!_timer.Enabled) StartTimer();
        }
        /// <summary>
        /// Starts the <see cref="FadeOutStoryboard"/> and makes the window start disappearing. Does nothing if the window isn't visible.
        /// </summary>
        private void StartFadeOut()
        {
            // if not visible OR currently fading out, do nothing
            if (!IsVisible || IsFadingOut) return;

            // window is visible AND not fading out

            // begin the storyboard animation
            this.Dispatcher.Invoke(() => FadeOutStoryboard.Begin(this, isControllable: true));
            // now fading out, set the fading out state to true
            IsFadingOut = true;
        }
        /// <summary>
        /// Interrupts the fade-out animation and makes the window fully opaque.
        /// </summary>
        private void StopFadeOut()
        {
            // if not visible OR not currently fading out, do nothing
            if (!IsVisible || !IsFadingOut) return;

            // window is visible AND currently fading out

            // stop the storyboard animation
            this.Dispatcher.Invoke(() => FadeOutStoryboard.Stop(this));
            // no longer fading out, set fading state to false
            IsFadingOut = false;
            // set opacity to fully opaque
            Opacity = 1.0;
        }
        #endregion Start/Stop Fade Animations

        #region ResetPosition
        public void ResetPosition()
        {
            this.SetPos(new Point(SystemParameters.WorkArea.Right - this.ActualWidth - 10, SystemParameters.WorkArea.Bottom - this.ActualHeight - 10));
        }
        #endregion ResetPosition

        #region Save/Load Position
        private bool SavePosition()
        {
            // if saving position is enabled and the notif window has actually loaded, save the current position
            if (Settings.NotificationSavePos && _loaded)
            {
                Dispatcher.Invoke(() =>
                {
                    VM.ConfigSection.PositionOriginCorner = this.GetCurrentScreenCorner();
                    VM.ConfigSection.Position = this.GetPosAtCorner(VM.ConfigSection.PositionOriginCorner);
                });
                return true;
            }
            else return false;
        }
        private bool LoadPosition()
        {
            // if saving position is enabled and there is a saved position to load
            if (Settings.NotificationSavePos && VM.ConfigSection.Position is Point pos)
            {
                Dispatcher.Invoke(() =>
                {
                    this.SetPosAtCorner(VM.ConfigSection.PositionOriginCorner, pos);
                });
                return true;
            }
            else return false;
        }
        #endregion Save/Load Position

        #endregion Methods

        #region Window Method Overrides
        protected override void OnRenderSizeChanged(SizeChangedInfo e)
        {
            base.OnRenderSizeChanged(e);

            if (!_loaded) return;

            // update the position of the window for the new size of the window
            switch (this.GetCurrentScreenCorner())
            {
            case EScreenCorner.TopLeft:
                break;
            case EScreenCorner.TopRight:
                if (!e.WidthChanged) return;

                this.Left += e.PreviousSize.Width - e.NewSize.Width;
                break;
            case EScreenCorner.BottomLeft:
                if (!e.HeightChanged) return;

                this.Top += e.PreviousSize.Height - e.NewSize.Height;
                break;
            case EScreenCorner.BottomRight:
                this.Left += e.PreviousSize.Width - e.NewSize.Width;
                this.Top += e.PreviousSize.Height - e.NewSize.Height;
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(Settings.DeviceListNotificationConfig.PositionOriginCorner), (byte)Settings.DeviceListNotificationConfig.PositionOriginCorner, typeof(EScreenCorner));
            }
        }
        #endregion Window Method Overrides

        #region EventHandlers

        #region _timer
        /// <summary>
        /// Hide the window, if the mouse is not over it and the keyboard is not focused on any subelements
        /// </summary>
        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsMouseOver)
            {
                this.Dispatcher.Invoke(() => RestartTimer()); //< restart the timer
                return;
            }

            Hide();
        }
        #endregion _timer

        #region DeviceListNotificationConfig
        /// <summary>
        /// Hides the window if the timeout is enabled while the window is already visible.
        /// This fixes a bug where the notification window won't automatically disappear.
        /// </summary>
        private void DeviceListNotificationConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (IsVisible && e.PropertyName != null)
            {
                if ((e.PropertyName.Equals(nameof(Settings.DeviceListNotificationConfig.DoFadeOut), StringComparison.Ordinal) && !_timer.Enabled)
                    || e.PropertyName.Equals(nameof(Settings.DeviceListNotificationConfig.Enabled)))
                {
                    Hide();
                }
            }
        }
        #endregion DeviceListNotificationConfig

        #region VCEvents
        /// <summary>
        /// Make the window appear (checking if notifications are enabled is unnecessary here)
        /// </summary>
        private void VCEvents_ShowDeviceListNotification(object? sender, EventArgs e)
            => Show();
        #endregion VCEvents

        #region ListView
        /// <summary>
        /// Prevents mouse events from being received by the ListView when the selection is locked
        /// </summary>
        private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.NotificationExtraMouseControlsEnabled)
            {
                if (e.ChangedButton.Equals(MouseButton.Middle))
                { // middle mouse toggles device lock
                    VCSettings.AudioAPI.AudioDeviceSelector.LockSelection = !VCSettings.AudioAPI.AudioDeviceSelector.LockSelection;
                    e.Handled = true;
                    return;
                }
                else if (!Settings.LockTargetDevice && e.ChangedButton.Equals(MouseButton.Right))
                { // right mouse deselects
                    VCSettings.AudioAPI.AudioDeviceSelector.DeselectDevice();
                    e.Handled = true;
                    return;
                }
            }
            if (Settings.LockTargetDevice)
            {
                e.Handled = true; //< ignore MouseDown events when selection is locked
            }
        }
        #endregion ListView

        #region ListViewItem
        /// <summary>
        /// Release mouse capture when a list item is selected and previously there was no selected item.
        /// This prevents the selection from accidentally changing when the device controls suddenly appear &amp; the window resizes.
        /// </summary>
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (VCAPI.Default.AudioDeviceSelector.Selected is null)
            { // only release mouse capture when there was no select
                ListView.ReleaseMouseCapture();
            }
        }
        #endregion ListViewItem

        #region Storyboards
        private void FadeInStoryboard_Completed(object sender, EventArgs e)
        {
            //< we are on the main UI thread here
            _fading = false;
            _fadingIn = false;
            StartTimer(); //< start the timer after the fade in animation has completed
        }
        private void FadeOutStoryboard_Completed(object sender, EventArgs e)
        {
            //< we are on the main UI thread here
            base.Hide();
            _fading = false;
            _fadingIn = false;
            this.Opacity = 1.0; //< reset Opacity property now that we're done with it; this fixes a bug when FadeIn is disabled.
            SavePosition();
        }
        #endregion Storyboards

        #region Window
        /// <summary>
        /// Stops the timeout timer when the mouse enters the window area
        /// </summary>
        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            StopTimer();
            if (_fading)
            {
                if (_fadingIn)
                {
                    StopFadeIn(resetOpacity: true);
                }
                else
                {
                    StopFadeOut();
                }
            }
        }
        /// <summary>
        /// Starts the timeout timer (if enabled) when the mouse leaves the window area
        /// </summary>
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            RestartTimer();
        }
        /// <summary>
        /// Allows dragging the window around with the mouse from anywhere overtop of it
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.ChangedButton.Equals(MouseButton.Left)
                || e.LeftButton != MouseButtonState.Pressed
                || (Settings.NotificationMoveRequiresAlt && !(Keyboard.PrimaryDevice.IsModifierKeyDown(EModifierKey.Alt))))
                return; //< don't do anything if the left mouse button wasn't pressed or if NotificationMoveRequiresAlt is enabled & ALT isn't pressed

            this.DragMove();
            e.Handled = true;
        }
        /// <summary>
        /// Allows dragging the window around with the mouse from anywhere (including on top of controls) when ALT is held down
        /// </summary>
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.ChangedButton.Equals(MouseButton.Left)
                || e.LeftButton != MouseButtonState.Pressed
                || !Keyboard.PrimaryDevice.IsModifierKeyDown(EModifierKey.Alt))
                return; //< don't do anything if the left mouse button wasn't pressed or if ALT isn't pressed

            this.DragMove();
            e.Handled = true;
        }
        /// <summary>
        /// Prevents controls from capturing the mouse &amp; preventing the Window's MouseLeave event from triggering
        /// </summary>
        private void Window_IsMouseCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && sender is FrameworkElement element)
            {
                element.ReleaseMouseCapture(); //< release mouse capture so MouseLeave will trigger properly
            }
        }
        /// <summary>
        /// Initializes the position of the window
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_loaded)
            {
                _loaded = true;

                if (!LoadPosition())
                { // couldn't load previous position, use default position instead
                    ResetPosition();
                }
            }
        }
        /// <summary>
        /// Saves the position of the notification window to the settings
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_allowClose) e.Cancel = true;
            SavePosition();
        }
        #endregion Window

        #region FlagsVM
        private void FlagsVM_StateChanged(object? sender, (ENotificationViewMode NewState, ENotificationViewMode ChangedFlags) e)
        {
            if (IsHiddenByViewMode)
                HideNowNoFadeOut();
        }
        #endregion FlagsVM

        #region AudioDeviceSelector
        private void AudioDeviceSelector_SelectedDeviceChanged(object? sender, CoreAudio.AudioDevice? e)
        {
            if (e != null) return;

            if (IsHiddenByViewMode)
                HideNowNoFadeOut();
        }
        #endregion AudioDeviceSelector

        #endregion EventHandlers
    }
}
