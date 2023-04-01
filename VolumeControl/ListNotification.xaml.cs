using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VolumeControl.Core;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.TypeExtensions;
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
            this.VCSettings.ListNotificationVM.PropertyChanged += this.ListNotificationVM_PropertyChanged;

            MainWindow.Closed += (s, e) =>
            {
                _allowClose = true;
                this.Close();
            };
        }
        private void lnotifWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_loaded)
            {
                _loaded = true;

                if (Settings.NotificationSavePos && Settings.NotificationPosition is Point pos)
                {
                    SetPosAtCorner(Settings.NotificationPositionOriginCorner, pos);
                }
                else
                {
                    SetPos(new(SystemParameters.WorkArea.Right - this.ActualWidth - 10, SystemParameters.WorkArea.Bottom - this.ActualHeight - 10));
                }
            }
        }
        #endregion Initializers

        #region Fields
        private readonly System.Timers.Timer _timer;
        private bool _allowClose = false;
        private bool _loaded = false;
        private bool _fading = false;
        private bool _fadingIn = false;
        #endregion Fields

        #region Properties
        private CompositionTarget? CompositionTarget => PresentationSource.FromVisual(this)?.CompositionTarget;
        private Storyboard FadeInStoryboard => (FindResource(nameof(FadeInStoryboard)) as Storyboard)!;
        private Storyboard FadeOutStoryboard => (FindResource(nameof(FadeOutStoryboard)) as Storyboard)!;
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
        {
            if (_timer.Enabled) this.StopTimer();
            if (!Settings.NotificationDoFadeIn)
            { // fade-in disabled:
                this.ForceShow();
            }
            else if (IsVisible && !_fadingIn)
            {
                this.Dispatcher.Invoke(() =>
                {
                    FadeOutStoryboard.Stop(this);
                    ClearValue(OpacityProperty);
                    ForceShow();
                    _fading = false;
                    _fadingIn = false;
                });
            }
            else if (!IsVisible)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Opacity = 0.0;
                    ForceShow();
                    _fading = true;
                    _fadingIn = true;
                    FadeInStoryboard.Begin(this, true);
                });
            }
            this.StartTimer();
        }
        public void ForceShow()
        {
            if (_timer.Enabled) this.StopTimer();
            this.Dispatcher.Invoke(base.Show);
            this.StartTimer();
        }
        public new void Hide()
        {
            if (!Settings.NotificationDoFadeOut)
            { // fade-out disabled:
                this.ForceHide();
            }
            else if (!_fading)
            {
                _fading = true;
                this.Dispatcher.Invoke(() => FadeOutStoryboard.Begin(this, true));
            }
        }
        public void ForceHide()
        {
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
        private void fadeOutWindowStoryboard_Completed(object sender, EventArgs e)
        {
            this.ForceHide();
            _fading = false;
        }
        private void fadeInWindowStoryboard_Completed(object sender, EventArgs e)
        {
            _fading = false;
            _fadingIn = false;
        }
        private void lnotifWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_fading)
            {
                FadeOutStoryboard.Stop(this);
                this.Show();
            }
        }
        private void ListNotificationVM_Show(object? sender, object e) => this.Show();
        private void ListNotificationVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not string name) return;

            if (name.Equals(nameof(ListNotificationVM.SelectedItemControls)))
            {
                selectedItemControlsTemplate.Children.Clear();
                if (VCSettings.ListNotificationVM.SelectedItemControls is Control[] controls)
                    AttachListDisplayTargetControls(selectedItemControlsTemplate, controls);
            }
        }

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
                    e.Handled = true;
                }
            }
        }
        private void lnotifWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_allowClose) e.Cancel = true;
            if (Settings.NotificationSavePos && _loaded)
            { // set the origin corner & position ; only when successfully loaded, otherwise we'll overwrite the notification position with garbage
                Settings.NotificationPositionOriginCorner = GetCurrentScreenCorner();
                Settings.NotificationPosition = GetPosAtCorner(Settings.NotificationPositionOriginCorner = GetCurrentScreenCorner());
            }
        }

        #region Positioning
        internal Point GetCenterPoint() => new(this.Left + (this.Width / 2), this.Top + (this.Height / 2));
        internal static System.Windows.Forms.Screen GetScreen(Point pos) => System.Windows.Forms.Screen.FromPoint(new((int)pos.X, (int)pos.Y));
        internal System.Windows.Forms.Screen GetCurrentScreen() => GetScreen(GetCenterPoint());
        internal static Point GetScreenCenterPoint(System.Windows.Forms.Screen screen) => new(screen.WorkingArea.Left + (screen.WorkingArea.Width / 2), screen.WorkingArea.Top + (screen.WorkingArea.Height / 2));
        internal Point GetCurrentScreenCenterPoint() => GetScreenCenterPoint(GetCurrentScreen());
        /// <summary>
        /// Gets the <see cref="Core.Helpers.ScreenCorner"/> that is closest to the given <paramref name="pos"/>, relative to <paramref name="screen"/>.
        /// </summary>
        /// <param name="screen">The screen to use. See <see cref="GetScreen(Point)"/>.</param>
        /// <param name="pos">A position on the <paramref name="screen"/>. This should be the centerpoint of the window, see <see cref="GetCenterPoint"/>.</param>
        /// <returns>The <see cref="Core.Helpers.ScreenCorner"/> that represents the corner of <paramref name="screen"/> that is closest to <paramref name="pos"/>.</returns>
        internal static Core.Helpers.ScreenCorner GetScreenCorner(System.Windows.Forms.Screen screen, Point pos)
        {
            // automatic corner selection is enabled:
            // get the centerpoint of this window
            (double cx, double cy) = GetScreenCenterPoint(screen);

            // figure out which corner is the closest & use that
            bool left = pos.X < cx;
            bool top = pos.Y < cy;

            if (left && top)
                return Core.Helpers.ScreenCorner.TopLeft;
            else if (!left && top)
                return Core.Helpers.ScreenCorner.TopRight;
            else if (left && !top)
                return Core.Helpers.ScreenCorner.BottomLeft;
            else if (!left && !top)
                return Core.Helpers.ScreenCorner.BottomRight;
            // else we're directly in the center; fallback to the value of Settings.NotificationWindowOriginCorner

            return 0;
        }
        /// <inheritdoc cref="GetScreenCorner(System.Windows.Forms.Screen, Point)"/>
        /// <remarks>
        /// This calls <see cref="GetScreenCorner(System.Windows.Forms.Screen, Point)"/> internally by automatically determining the screen to use based on the given <paramref name="pos"/>.
        /// </remarks>
        internal static Core.Helpers.ScreenCorner GetScreenCorner(Point pos) => GetScreenCorner(GetScreen(pos), pos);
        /// <summary>
        /// Gets the closest <see cref="Core.Helpers.ScreenCorner"/> to the window's centerpoint.
        /// </summary>
        /// <returns>The closest <see cref="Core.Helpers.ScreenCorner"/> to the window's centerpoint.</returns>
        internal Core.Helpers.ScreenCorner GetCurrentScreenCorner() => GetScreenCorner(GetCenterPoint());
        /// <summary>
        /// Gets the position of the window at the specified <paramref name="corner"/>.
        /// </summary>
        /// <param name="corner">The corner to check</param>
        /// <returns>The position of the window at the specified <paramref name="corner"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Invalid <see cref="Core.Helpers.ScreenCorner"/> enumeration.</exception>
        internal Point GetPosAtCorner(Core.Helpers.ScreenCorner corner) => corner switch
        {
            Core.Helpers.ScreenCorner.TopLeft => CompositionTarget?.TransformToDevice.Transform(new Point(this.Left, this.Top)) ?? new Point(this.Left, this.Top),
            Core.Helpers.ScreenCorner.TopRight => CompositionTarget?.TransformToDevice.Transform(new Point(this.Left + this.ActualWidth, this.Top)) ?? new Point(this.Left + this.ActualWidth, this.Top),
            Core.Helpers.ScreenCorner.BottomLeft => CompositionTarget?.TransformToDevice.Transform(new Point(this.Left, this.Top + this.ActualHeight)) ?? new Point(this.Left, this.Top + this.ActualHeight),
            Core.Helpers.ScreenCorner.BottomRight => CompositionTarget?.TransformToDevice.Transform(new Point(this.Left + this.ActualWidth, this.Top + this.ActualHeight)) ?? new Point(this.Left + this.ActualWidth, this.Top + this.ActualHeight),
            _ => throw new InvalidEnumArgumentException(nameof(corner), (int)corner, typeof(Core.Helpers.ScreenCorner)),
        };
        internal Point GetPosAtCurrentCorner() => GetPosAtCorner(GetCurrentScreenCorner());
        internal void SetPosAtCorner(Core.Helpers.ScreenCorner corner, Point pos)
        {
            switch (corner)
            {
            case Core.Helpers.ScreenCorner.TopLeft:
                this.Left = pos.X;
                this.Top = pos.Y;
                break;
            case Core.Helpers.ScreenCorner.TopRight:
                this.Left = pos.X + this.Width;
                this.Top = pos.Y;
                break;
            case Core.Helpers.ScreenCorner.BottomLeft:
                this.Left = pos.X;
                this.Top = pos.Y - this.Height;
                break;
            case Core.Helpers.ScreenCorner.BottomRight:
                this.Left = pos.X + this.Width;
                this.Top = pos.Y - this.Height;
                break;
            }
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo e)
        {
            base.OnRenderSizeChanged(e);

            if (!_loaded) return;

            switch (GetCurrentScreenCorner())
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
                throw new InvalidEnumArgumentException(nameof(Settings.NotificationPositionOriginCorner), (byte)Settings.NotificationPositionOriginCorner, typeof(Core.Helpers.ScreenCorner));
            }
        }
        #endregion Positioning

        private static void AttachListDisplayTargetControls(StackPanel stack, Control[] controls)
        {
            foreach (Control? control in controls)
            {
                // disconnect the control from its parent before trying to attach it
                //if (control.Parent is Panel parentPanel)
                //{
                //    parentPanel.Children.Remove(control);
                //}
                try
                {
                    _ = stack.Children.Add(control);
                }
                catch (Exception ex)
                {
                    Log.Error($"Attaching templated {nameof(Control)} of type {control.GetType().FullName} caused an exception!", ex);
                }
            }
            stack.UpdateLayout();
        }
        /// <summary>Adds all custom controls to the calling stackpanel</summary>
        private void displayableControlsTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel stack)
            {
                //const string listControlName = "displayableControlsTemplate";
                //const string selectedControlName = "selectedItemControlsTemplate";
                //if (stack.Name.Equals(listControlName, StringComparison.Ordinal))
                //{
                //    if (!Settings.NotificationShowsCustomControls) return;
                //}
                //else if (stack.Name.Equals(selectedControlName, StringComparison.Ordinal))
                //{
                //    if (Settings.NotificationShowsCustomControls) return;
                //}
                if (stack.Tag is Control[] arr)
                {
                    AttachListDisplayTargetControls(stack, arr);
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
                    AttachListDisplayTargetControls(stack, arr);
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
