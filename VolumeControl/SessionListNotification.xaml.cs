using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.SDK.Internal;
using VolumeControl.ViewModels;
using VolumeControl.WPF;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for SessionListNotification.xaml
    /// </summary>
    public partial class SessionListNotification : Window
    {
        public SessionListNotification()
        {
            InitializeComponent();

            // setup the timeout timer
            if (Settings.SessionListNotificationConfig.TimeoutMs <= 0)
            { // validate the timeout value before using it for the timer interval to prevent possible unhandled exception
                int defaultValue = NotificationConfigSection.DefaultTimeoutMs;
                Log.Error($"{nameof(SessionListNotification)} {nameof(Settings.SessionListNotificationConfig.TimeoutMs)} cannot be less than or equal to zero; it was reset to '{defaultValue}' in order to avoid a fatal exception.",
                    new ArgumentOutOfRangeException($"{nameof(Settings)}.{nameof(Settings.SessionListNotificationConfig)}.{nameof(Settings.SessionListNotificationConfig.TimeoutMs)}", Settings.SessionListNotificationConfig.TimeoutMs, $"The value '{Settings.SessionListNotificationConfig.TimeoutMs}' isn't valid for property 'System.Timers.Timer.Interval'; Value is out-of-range! (Minimum: 1)"));
                Settings.SessionListNotificationConfig.TimeoutMs = defaultValue;
            }
            _timer = new()
            {
                AutoReset = false,
            };
            // bind the interval
            _timer.SetBinding(BindableTimer.IntervalProperty, new Binding()
            {
                Source = Settings,
                Path = new PropertyPath($"{nameof(Config.SessionListNotificationConfig)}.{nameof(NotificationConfigSection.TimeoutMs)}"),
                Mode = BindingMode.OneWay
            });
            _timer.Elapsed += this._timer_Elapsed;

            // close the window when the main window closes
            App.Current.MainWindow.Closed += (s, e) =>
            {
                _allowClose = true; //< must be true for this window to be able to close
                this.Close();
            };

            Settings.SessionListNotificationConfig.PropertyChanged += this.SessionListNotificationConfig_PropertyChanged;

            // bind to the show event
            VCEvents.ShowSessionListNotification += this.VCEvents_ShowSessionListNotification;
        }

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
        private static Config Settings => (AppConfig.Configuration.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        public VolumeControlVM VCSettings => (this.FindResource("Settings") as VolumeControlVM)!;
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
        #endregion Properties

        #region Methods
        #region Start/Stop-Timer
        /// <summary>
        /// Starts the timer if <see cref="Config.NotificationTimeoutEnabled"/> is <see langword="true"/>; otherwise does nothing.
        /// </summary>
        private void StartTimer()
        {
            if (!Settings.SessionListNotificationConfig.TimeoutEnabled) return;
            _timer.Start();
        }
        /// <summary>
        /// Stops the <see cref="_timer"/>, preventing the <see cref="System.Timers.Timer.Elapsed"/> event from firing.
        /// </summary>
        private void StopTimer() => _timer.Stop();
        private void RestartTimer()
        {
            if (!Settings.SessionListNotificationConfig.TimeoutEnabled) return;
            _timer.Restart();
        }
        #endregion Start/Stop-Timer

        #region Show/Hide
        public new void Show()
        {
            StopTimer();
            if (!Settings.SessionListNotificationConfig.DoFadeIn)
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
            if (!Settings.SessionListNotificationConfig.DoFadeOut)
            { // fade-out disabled:
                //< we are NOT on the main UI thread here (!), use the dispatcher:
                this.Dispatcher.Invoke(() =>
                {
                    StopTimer();
                    base.Hide();
                });
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
                throw new InvalidEnumArgumentException(nameof(Settings.SessionListNotificationConfig.PositionOriginCorner), (byte)Settings.SessionListNotificationConfig.PositionOriginCorner, typeof(EScreenCorner));
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
            if (IsMouseOver || IsKeyboardFocusWithin)
            {
                this.Dispatcher.Invoke(() => RestartTimer()); //< restart the timer
                return;
            }

            Hide();
        }
        #endregion _timer

        #region SessionListNotificationConfig
        private void SessionListNotificationConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (IsVisible && e.PropertyName != null)
            {
                if ((e.PropertyName.Equals(nameof(NotificationConfigSection.DoFadeOut)) && !_timer.Enabled)
                    || e.PropertyName.Equals(nameof(NotificationConfigSection.Enabled)))
                {
                    Hide();
                }
            }
        }
        #endregion SessionListNotificationConfig

        #region VCEvents
        /// <summary>
        /// Make the window appear (checking if notifications are enabled is unnecessary here)
        /// </summary>
        private void VCEvents_ShowSessionListNotification(object? sender, EventArgs e)
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
                { // middle mouse toggles session lock
                    VCSettings.AudioAPI.AudioSessionMultiSelector.LockSelection = !VCSettings.AudioAPI.AudioSessionMultiSelector.LockSelection;
                    e.Handled = true;
                    return;
                }
                else if (e.ChangedButton.Equals(MouseButton.Right))
                { // right mouse deselects
                    if (!Settings.LockTargetSession)
                    {
                        VCSettings.AudioAPI.AudioSessionMultiSelector.UnsetCurrentIndex();
                    }
                    e.Handled = true;
                    return;
                }
            }
            if (Settings.LockTargetSession)
            {
                e.Handled = true; //< ignore MouseDown events when selection is locked
            }
        }
        #endregion ListView

        #region ListViewItem
        /// <summary>
        /// Release mouse capture when a list item is selected and previously there was no selected item.
        /// This prevents the selection from accidentally changing when the session controls suddenly appear &amp; the window resizes.
        /// </summary>
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ListView.ReleaseMouseCapture();
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
            if (IsKeyboardFocusWithin) return;
            RestartTimer();
        }
        /// <summary>
        /// Allows dragging the window around with the mouse from anywhere overtop of it
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.ChangedButton.Equals(MouseButton.Left)) return;

            if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
            {
                if (Settings.NotificationMoveRequiresAlt && !(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                    return;
                this.DragMove();
                e.Handled = true;
            }
        }
        /// <summary>
        /// stops the timer when keyboard focus was acquired
        /// </summary>
        private void Window_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            StopTimer();
        }
        /// <summary>
        /// (re)starts the timer when keyboard focus was lost
        /// </summary>
        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsMouseOver) return;
            RestartTimer();
        }
        /// <summary>
        /// Initializes the position of the window
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_loaded)
            {
                _loaded = true;

                if (Settings.NotificationSavePos && Settings.SessionListNotificationConfig.Position is Point pos)
                {
                    this.SetPosAtCorner(Settings.SessionListNotificationConfig.PositionOriginCorner, pos);
                }
                else
                {
                    this.SetPos(new(SystemParameters.WorkArea.Right - this.ActualWidth - 10, SystemParameters.WorkArea.Bottom - this.ActualHeight - 10));
                }
            }
        }
        /// <summary>
        /// Saves the position of the notification window to the settings
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_allowClose) e.Cancel = true;
            // if saving position is enabled and the notif window has actually loaded, save the current position
            if (Settings.NotificationSavePos && _loaded)
            {
                Settings.SessionListNotificationConfig.PositionOriginCorner = this.GetCurrentScreenCorner();
                Settings.SessionListNotificationConfig.Position = this.GetPosAtCorner(Settings.SessionListNotificationConfig.PositionOriginCorner);
            }
        }
        #endregion Window

        #endregion EventHandlers
    }
}
