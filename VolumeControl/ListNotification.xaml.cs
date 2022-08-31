using SVGImage.SVG;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VolumeControl.Core;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.ViewModels;
using VolumeControl.WPF;

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
            this.InitializeComponent();

            if (Settings.NotificationSavePos && Settings.NotificationPosition.HasValue)
                SetPos(Settings.NotificationPosition.Value);
            else // use the default location
                SetPos(new(SystemParameters.WorkArea.Right - this.Width - 10, SystemParameters.WorkArea.Bottom - this.Height - 10));

            Settings.PropertyChanged += Settings_PropertyChanged;

            if (Settings.NotificationTimeoutMs <= 0)
            { // validate the timeout value before using it for the timer interval
                const int resetToValue = 3000;
                Log.Error($"{nameof(Settings.NotificationTimeoutMs)} cannot be less than or equal to zero; it was reset to '{resetToValue}' in order to avoid a fatal exception.",
                    new ArgumentOutOfRangeException($"{nameof(Settings)}.{nameof(Settings.NotificationTimeoutMs)}", Settings.NotificationTimeoutMs, $"The value '{Settings.NotificationTimeoutMs}' isn't valid for property 'System.Timers.Timer.Interval'; Value is out-of-range! (Minimum: 1)"));
                Settings.NotificationTimeoutMs = resetToValue;
            }
            _timer = new()
            {
                Interval = Settings.NotificationTimeoutMs,
                AutoReset = false,
            };
            _timer.Elapsed += Timer_Elapsed;

            VCSettings.ListNotificationVM.Show += ListNotificationVM_Show;
        }
        #endregion Initializers

        #region Fields
        private readonly System.Timers.Timer _timer;
        #endregion Fields

        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        private VolumeControlSettings? _vcSettings;
        private VolumeControlSettings VCSettings => _vcSettings ??= (FindResource("Settings") as VolumeControlSettings)!;
        private ListDisplayTarget? CurrentDisplayTarget => VCSettings.ListNotificationVM.CurrentDisplayTarget;

        /// <summary>
        /// Starts the timer if <see cref="Config.NotificationTimeoutEnabled"/> is <see langword="true"/>; otherwise does nothing.
        /// </summary>
        private void StartTimer()
        {
            if (!Settings.NotificationTimeoutEnabled) return;
            _timer.Start();
        }
        private void StopTimer() => _timer.Stop();

        public new void Show()
        {
            SVG svg = new();
            if (_timer.Enabled) StopTimer();
            Dispatcher.Invoke(base.Show);
            StartTimer();
        }
        public new void Hide()
        {
            StopTimer();
            Dispatcher.Invoke(base.Hide);
        }

        private Point GetPos() => new(this.Left, this.Top);
        private void SetPos(Point p)
        {
            this.Left = p.X;
            this.Top = p.Y;
        }

        private void ListNotificationVM_Show(object? sender, object e) => this.Show();

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (this.IsMouseOver || this.HasEffectiveKeyboardFocus)
                    StartTimer();
                else
                    this.Hide();
            });
        }

        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (name is null) return;

            if (name.Equals(nameof(Config.NotificationTimeoutMs)))
            {
                _timer.Interval = Settings.NotificationTimeoutMs;
            }
            else if (name.Equals(nameof(Config.NotificationTimeoutEnabled)))
            {
                if (_timer.Enabled)
                    StopTimer();
                else
                    StartTimer();
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
            Settings.NotificationPosition = GetPos();
        }
        private void lnotifWindow_Closing(object sender, CancelEventArgs e) => e.Cancel = true;
        private void lnotifWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var corner = Settings.NotificationWindowOriginCorner;
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
                    scr.WorkingArea.Left + scr.WorkingArea.Width / 2,
                    scr.WorkingArea.Top + scr.WorkingArea.Height / 2
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
            foreach (var control in controls)
            {
                try
                {
                    stack.Children.Add(control);
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
    }
}
