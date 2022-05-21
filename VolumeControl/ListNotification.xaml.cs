using System;
using System.Windows;
using VolumeControl.Extensions;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for ListNotification.xaml
    /// </summary>
    public partial class ListNotification : Window
    {
        #region Initializers
        public ListNotification()
        {
            InitializeComponent();

            // Apply settings
            _enabled = Settings.NotificationEnabled;
            TimeoutTimer.Interval = Settings.NotificationTimeoutInterval;

            // Add an event handler
            TimeoutTimer.Tick += Handle_TimeoutTimerTick!;
        }
        #endregion Initializers

        #region Finalizers
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Apply settings
            Settings.NotificationEnabled = _enabled;
            Settings.NotificationTimeoutInterval = TimeoutTimer.Interval;
            // Save settings
            Settings.Save();
            Settings.Reload();
        }
        #endregion Finalizers

        #region Fields
        public readonly System.Windows.Forms.Timer TimeoutTimer = new() { Enabled = false };
        private bool _enabled = false;
        #endregion Fields

        #region Properties
        private static Properties.Settings Settings => Properties.Settings.Default;
        public decimal TimeoutInterval
        {
            get => TimeoutTimer.Interval;
            set => TimeoutTimer.Interval = Convert.ToInt32(value);
        }
        #endregion Properties

        #region Methods
        public new void Show()
        {
            if (!_enabled)
                return;

            base.Show();
            TimeoutTimer.StartOrReset();
        }
        public new void Hide()
        {
            TimeoutTimer.Stop();
            base.Hide();
        }
        #endregion Methods

        #region EventHandlers
        private void Handle_TimeoutTimerTick(object sender, EventArgs e) => Hide();
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = true;
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rect = SystemParameters.WorkArea;
            Left = rect.Right - ActualWidth - 10;
            Top = rect.Bottom - ActualHeight - 10;
        }
        private void ControlGotFocus(object sender, RoutedEventArgs e) => TimeoutTimer.Stop();
        private void ControlLostFocus(object sender, RoutedEventArgs e) => TimeoutTimer.Start();
        private void ControlMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => TimeoutTimer.Stop();
        private void ControlMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => TimeoutTimer.Start();
        private void ControlDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e) => TimeoutTimer.Stop();
        private void ControlDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) => TimeoutTimer.Start();
        private void ControlGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) => TimeoutTimer.Stop();
        private void ControlLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) => TimeoutTimer.Start();
        #endregion EventHandlers

        #region DependencyProperties
        #region Enabled
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(ListNotification), new(false, OnEnabledChanged));
        public bool Enabled
        {
            get => (bool)GetValue(EnabledProperty);
            set => SetValue(EnabledProperty, _enabled = value);
        }
        private static void OnEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
            => ((ListNotification)element).Enabled = (bool)e.NewValue;
        #endregion Enabled

        #region SliderVisibility
        public static readonly DependencyProperty SliderVisibilityProperty = DependencyProperty.Register("SliderVisibility", typeof(Visibility), typeof(ListNotification), new(Visibility.Visible, OnSliderVisibilityChanged));
        public Visibility SliderVisibility
        {
            get => (Visibility)GetValue(SliderVisibilityProperty);
            set => SetValue(SliderVisibilityProperty, value);
        }
        private static void OnSliderVisibilityChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
            => ((ListNotification)element).SliderVisibility = (Visibility)e.NewValue;
        #endregion SliderVisibility

        #region SliderValue
        public static readonly DependencyProperty SliderValueProperty = DependencyProperty.Register("SliderValue", typeof(double), typeof(ListNotification), new(0.0, OnSliderValueChanged));
        public double SliderValue
        {
            get => slider.Value = Convert.ToDouble(GetValue(SliderValueProperty));
            set => SetValue(SliderValueProperty, slider.Value = value);
        }
        private static void OnSliderValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
            => ((ListNotification)element).SliderValue = (double)e.NewValue;
        #endregion SliderValue

        #region ValueText
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register("ValueText", typeof(string), typeof(ListNotification), new(string.Empty, OnValueTextChanged));
        public string? ValueText
        {
            get => Convert.ToString(GetValue(ValueTextProperty));
            set => SetValue(ValueTextProperty, value);
        }
        private static void OnValueTextChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
            => ((ListNotification)element).ValueText = Convert.ToString(e.NewValue);
        #endregion ValueText

        #region LockSelection
        public static readonly DependencyProperty LockSelectionProperty = DependencyProperty.Register("LockSelection", typeof(bool), typeof(ListNotification), new(false, OnLockSelectionChanged));
        public bool LockSelection
        {
            get => Convert.ToBoolean(GetValue(LockSelectionProperty));
            set => SetValue(LockSelectionProperty, value);
        }
        private static void OnLockSelectionChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
            => ((ListNotification)element).LockSelection = (bool)e.NewValue;
        #endregion LockSelection

        #region ShowIcons
        public static readonly DependencyProperty ShowIconsProperty = DependencyProperty.Register("ShowIcons", typeof(bool), typeof(ListNotification), new(Settings.ShowIcons, OnShowIconsChanged));
        public bool ShowIcons
        {
            get => Convert.ToBoolean(GetValue(ShowIconsProperty));
            set => SetValue(ShowIconsProperty, value);
        }
        private static void OnShowIconsChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
            => ((ListNotification)element).ShowIcons = Convert.ToBoolean(e.NewValue);
        #endregion ShowIcons
        #endregion DependencyProperties
    }
}
