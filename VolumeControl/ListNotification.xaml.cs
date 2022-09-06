using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VolumeControl.Core;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.ViewModels;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for ListNotification.xaml
    /// </summary>
    public partial class ListNotification : Window, ISupportInitialize
    {
        #region Initializers
        public ListNotification()
        {
            Style s = new()
            {
                TargetType = typeof(Window)
            };

            this.Resources.Add(typeof(Window), s);

            this.InitializeComponent();

            // set the window position
            if (Settings.NotificationSavePos && Settings.NotificationPosition.HasValue)
                this.SetPos(Settings.NotificationPosition.Value);
            else // use the default location
                this.SetPos(new(SystemParameters.WorkArea.Right - this.Width - 10, SystemParameters.WorkArea.Bottom - this.Height - 10));


            // create the timeout timer instance
            if (Settings.NotificationTimeoutMs <= 0)
            { // validate the timeout value before using it for the timer interval
                int defaultValue = new Config().NotificationTimeoutMs;
                Log.Error($"{nameof(Settings.NotificationTimeoutMs)} cannot be less than or equal to zero; it was reset to '{defaultValue}' in order to avoid a fatal exception.",
                    new ArgumentOutOfRangeException($"{nameof(Settings)}.{nameof(Settings.NotificationTimeoutMs)}", Settings.NotificationTimeoutMs, $"The value '{Settings.NotificationTimeoutMs}' isn't valid for property 'System.Timers.Timer.Interval'; Value is out-of-range! (Minimum: 1)"));
                Settings.NotificationTimeoutMs = defaultValue;
            }
            (_timer = new()
            {
                Interval = Settings.NotificationTimeoutMs,
                AutoReset = false,
            }).Elapsed += this.Timer_Elapsed;

            Settings.PropertyChanged += this.Settings_PropertyChanged;

            this.VCSettings.ListNotificationVM.Show += this.ListNotificationVM_Show;

            MainWindow.Closed += (s, e) =>
            {
                _allowClose = true;
                this.Close();
            };
        }
        #endregion Initializers

        #region Fields
        private readonly System.Timers.Timer _timer;
        private bool _allowClose = false;
        #endregion Fields

        #region Properties
        private static Window MainWindow => App.Current.MainWindow;
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        private VolumeControlSettings? _vcSettings;
        /// <summary>The <see cref="VolumeControlSettings"/> resource instance.</summary>
        private VolumeControlSettings VCSettings => _vcSettings ??= (this.FindResource("Settings") as VolumeControlSettings)!;
        /// <summary>The currently-selected <see cref="ListDisplayTarget"/> instance.</summary>
        private ListDisplayTarget? CurrentDisplayTarget => this.VCSettings.ListNotificationVM.CurrentDisplayTarget;
        #endregion Properties

        #region Methods
        #region Start/Stop-Timer
        /// <summary>
        /// Starts the timer if <see cref="Config.NotificationTimeoutEnabled"/> is <see langword="true"/>; otherwise does nothing.
        /// </summary>
        private void StartTimer()
        {
            if (!Settings.NotificationTimeoutEnabled) return;
            _timer.Start();
        }
        /// <summary>
        /// Stops the <see cref="_timer"/>, preventing the <see cref="System.Timers.Timer.Elapsed"/> event from firing.
        /// </summary>
        private void StopTimer() => _timer.Stop();
        #endregion Start/Stop-Timer

        #region Show/Hide
        public new void Show()
        { // override the show method to add timer controls
            if (_timer.Enabled) this.StopTimer();
            this.Dispatcher.Invoke(base.Show);
            this.StartTimer();
        }
        public new void Hide()
        { // override the hide method to add timer controls
            this.StopTimer();
            this.Dispatcher.Invoke(base.Hide);
        }
        #endregion Show/Hide

        #region Get/Set-Pos
        /// <summary>
        /// Gets the position of this <see cref="ListNotification"/> window.
        /// </summary>
        /// <returns>The window's position as a <see cref="Point"/></returns>
        private Point GetPos() => new(this.Left, this.Top);
        /// <summary>
        /// Sets the position of this <see cref="ListNotification"/> window to the given <see cref="Point"/>, <paramref name="p"/>.
        /// </summary>
        /// <param name="p">A <see cref="Point"/> <see langword="struct"/> specifying a position in screen-space coordinates.</param>
        private void SetPos(Point p)
        {
            this.Left = p.X;
            this.Top = p.Y;
        }
        #endregion Get/Set-Pos
        #endregion Methods

        #region EventHandlers
        private void ListNotificationVM_Show(object? sender, object e) => this.Show();

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) => this.Dispatcher.Invoke(() =>
                                                                                                 {
                                                                                                     if (this.IsMouseOver || this.HasEffectiveKeyboardFocus)
                                                                                                         this.StartTimer();
                                                                                                     else
                                                                                                         this.Hide();
                                                                                                 });

        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            string? name = e.PropertyName;
            if (name is null) return;

            if (name.Equals(nameof(Config.NotificationTimeoutMs)))
            {
                _timer.Interval = Settings.NotificationTimeoutMs;
            }
            else if (name.Equals(nameof(Config.NotificationTimeoutEnabled)))
            {
                if (_timer.Enabled)
                    this.StopTimer();
                else
                    this.StartTimer();
            }
        }

        /// <summary>Handler that allows dragging the notification window</summary>
        private void lnotifWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.ChangedButton.Equals(MouseButton.Left)) return;

            if (!Settings.NotificationMoveRequiresAlt || Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (Mouse.LeftButton.Equals(MouseButtonState.Pressed))
                {
                    this.DragMove(); //< apparently this throws an exception if you release the mouse button really fast
                }
            }
        }
        /// <summary>Saves the position of the notification window, if enabled by the config.</summary>
        private void lnotifWindow_LocationChanged(object sender, EventArgs e)
        {
            if (!Settings.NotificationSavePos) return;
            Settings.NotificationPosition = this.GetPos();
        }
        private void lnotifWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_allowClose) e.Cancel = true;
        }
        private void lnotifWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Core.Helpers.ScreenCorner corner = Settings.NotificationWindowOriginCorner;
            if (Settings.NotificationWindowOriginCornerAuto)
            { // automatic corner selection is enabled:
                // get the centerpoint of this window
                double
                    x = this.Left + (e.PreviousSize.Width / 2),
                    y = this.Top + (e.PreviousSize.Height / 2);
                //                  floor via truncation   vvv     vvv
                var scr = System.Windows.Forms.Screen.FromPoint(new((int)x, (int)y));

                // get the centerpoint of this screen
                var center = new Point(
                    scr.WorkingArea.Left + (scr.WorkingArea.Width / 2),
                    scr.WorkingArea.Top + (scr.WorkingArea.Height / 2)
                    );

                // figure out which corner is the closest & use that
                bool left = x < center.X, top = y < center.Y;

                if (left && top)
                    corner = Core.Helpers.ScreenCorner.TopLeft;
                else if (!left && top)
                    corner = Core.Helpers.ScreenCorner.TopRight;
                else if (left && !top)
                    corner = Core.Helpers.ScreenCorner.BottomLeft;
                else if (!left && !top)
                    corner = Core.Helpers.ScreenCorner.BottomRight;
                // else we're directly in the center; fallback to the value of Settings.NotificationWindowOriginCorner
            }
            switch (corner)
            {
            case Core.Helpers.ScreenCorner.TopLeft:
                break;
            case Core.Helpers.ScreenCorner.TopRight:
                if (!e.WidthChanged) return;

                this.Left += e.PreviousSize.Width - e.NewSize.Width;
                break;
            case Core.Helpers.ScreenCorner.BottomLeft:
                if (!e.HeightChanged) return;

                this.Top += e.PreviousSize.Height - e.NewSize.Height;
                break;
            case Core.Helpers.ScreenCorner.BottomRight:
                this.Left += e.PreviousSize.Width - e.NewSize.Width;
                this.Top += e.PreviousSize.Height - e.NewSize.Height;
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(Settings.NotificationWindowOriginCorner), (byte)Settings.NotificationWindowOriginCorner, typeof(Core.Helpers.ScreenCorner));
            }
        }

        private static void AttachListDisplayTargetControlsToStack(StackPanel stack, Control[] controls)
        {
            foreach (Control? control in controls)
            {
                try
                {
                    _ = stack.Children.Add(control);
                }
                catch (Exception ex)
                {
                    Log.Error($"Attaching templated {nameof(Control)} of type {control.GetType().FullName} caused an exception!", ex);
                }
            }
        }
        /// <summary>Adds all custom controls to the calling stackpanel</summary>
        private void displayableControlsTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel stack)
            {
                if (stack.Tag is Control[] arr)
                {
                    AttachListDisplayTargetControlsToStack(stack, arr);
                }
            }
        }
        /// <summary>Removes all custom controls from the calling stackpanel</summary>
        private void displayableControlsTemplate_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel stack)
            {
                stack.Children.Clear();
                stack.UpdateLayout();

                if (stack.Tag is Control[] arr)
                { // re-attach the controls:
                    AttachListDisplayTargetControlsToStack(stack, arr);
                }
            }
        }

        private void listView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.LockTargetSession)
            {
                e.Handled = true;
            }
        }
        #endregion EventHandlers
    }
}
