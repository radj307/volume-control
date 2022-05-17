using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for ListNotification.xaml
    /// </summary>
    public partial class ListNotification : Window
    {
        public ListNotification()
        {
            InitializeComponent();
            _timeoutTimer.Tick += Handle_TimeoutTimerTick!;
        }

        private readonly System.Windows.Forms.Timer _timeoutTimer = new() { Enabled = false };
        private bool _enabled = false;

        public new void Show()
        {
            if (!_enabled)
                return;

            base.Show();
            _timeoutTimer.Start();
        }
        public new void Hide()
        {
            _timeoutTimer.Stop();
            base.Hide();
        }

        private void Handle_TimeoutTimerTick(object sender, EventArgs e) => Hide();
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rect = SystemParameters.WorkArea;
            Left = rect.Right - ActualWidth - 10;
            Top = rect.Bottom - ActualHeight - 10;
        }

        #region DependencyProperties
        #region Enabled
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(ListNotification), new(false, OnEnabledChanged));
        public bool Enabled
        {
            get => (bool)GetValue(EnabledProperty);
            set => SetValue(EnabledProperty, _enabled = value);
        }
        private static void OnEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is ListNotification lnotif)
            {
                lnotif._enabled = lnotif.Enabled = (bool)e.NewValue;
            }
        }
        #endregion Enabled

        #region SliderVisibility
        public static readonly DependencyProperty SliderVisibilityProperty = DependencyProperty.Register("SliderVisibility", typeof(Visibility), typeof(ListNotification), new(Visibility.Visible, OnSliderVisibilityChanged));
        public Visibility SliderVisibility
        {
            get => (Visibility)GetValue(SliderVisibilityProperty);
            set => SetValue(SliderVisibilityProperty, value);
        }
        private static void OnSliderVisibilityChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is ListNotification lnotif && lnotif.sliderGrid != null)
            {
                lnotif.SliderVisibility = (Visibility)e.NewValue;
            }
        }
        #endregion SliderVisibility

        #region SliderValue
        public static readonly DependencyProperty SliderValueProperty = DependencyProperty.Register("SliderValue", typeof(double), typeof(ListNotification), new(0.0, OnSliderValueChanged));
        public double SliderValue
        {
            get => System.Convert.ToDouble(GetValue(SliderValueProperty));
            set => SetValue(SliderValueProperty, value);
        }
        private static void OnSliderValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is ListNotification lnotif && lnotif.slider != null)
            {
                lnotif.SliderValue = (double)e.NewValue;
            }
        }
        #endregion SliderValue

        #region ValueText
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register("ValueText", typeof(string), typeof(ListNotification), new(string.Empty, OnValueTextChanged));
        public string? ValueText
        {
            get => System.Convert.ToString(GetValue(ValueTextProperty));
            set => SetValue(ValueTextProperty, value);
        }
        private static void OnValueTextChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is ListNotification lnotif && lnotif.valueBox != null)
            {
                lnotif.ValueText = System.Convert.ToString(e.NewValue);
            }
        }
        #endregion ValueText

        #region LockSelection
        public static readonly DependencyProperty LockSelectionProperty = DependencyProperty.Register("LockSelection", typeof(bool), typeof(ListNotification), new(false, OnLockSelectionChanged));
        public bool LockSelection
        {
            get => System.Convert.ToBoolean(GetValue(LockSelectionProperty));
            set => SetValue(LockSelectionProperty, value);
        }
        private static void OnLockSelectionChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is ListNotification lnotif)
            {
                lnotif.LockSelection = System.Convert.ToBoolean(e.NewValue);
            }
        }
        #endregion LockSelection

        #region Timeout
        public static readonly DependencyProperty TimeoutProperty = DependencyProperty.Register("Timeout", typeof(int), typeof(ListNotification), new(3000, OnTimeoutChanged));
        public int Timeout
        {
            get => System.Convert.ToInt32(GetValue(TimeoutProperty));
            set => SetValue(TimeoutProperty, value);
        }
        private static void OnTimeoutChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is ListNotification lnotif)
            {
                lnotif.Timeout = lnotif._timeoutTimer.Interval = System.Convert.ToInt32(e.NewValue);
            }
        }
        #endregion Timeout
        #endregion DependencyProperties
    }
}
