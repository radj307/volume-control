using System;
using System.ComponentModel;
using System.Windows;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Helpers;
using VolumeControl.TypeExtensions;

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

            // Apply settings
            TimeoutTimer.Interval = Settings.NotificationTimeoutMs;
            this.VCSettings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == null)
                {
                    return;
                }

                if (e.PropertyName.Equals("NotificationTimeout"))
                {
                    TimeoutTimer.Interval = this.VCSettings.NotificationTimeout;
                }
                else if (e.PropertyName.Equals("NotificationEnabled") && !this.VCSettings.NotificationEnabled)
                {
                    this.Hide();
                }
            };

            // Add an event handler
            TimeoutTimer.Tick += this.Handle_TimeoutTimerTick!;
        }
        /// <inheritdoc/>
        public override void EndInit()
        {
            base.EndInit();
            listView.Style = this.FindResource("ListViewStyle") as Style;
        }
        #endregion Initializers

        #region Finalizers
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Apply settings
            Settings.NotificationTimeoutMs = TimeoutTimer.Interval;
            // Save settings
            Settings.Save();
        }
        #endregion Finalizers

        #region Fields
        public readonly System.Windows.Forms.Timer TimeoutTimer = new() { Enabled = false };
        private bool _mouseOver = false;
        #endregion Fields

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        private VolumeControlSettings VCSettings => _vcSettings ??= (this.FindResource("Settings") as VolumeControlSettings)!;
        private VolumeControlSettings? _vcSettings = null;
        /// <summary>Controls the amount of time <i>(in milliseconds)</i> that the list notification is visible for before disappearing.</summary>
        public decimal TimeoutInterval
        {
            get => TimeoutTimer.Interval;
            set => TimeoutTimer.Interval = Convert.ToInt32(value);
        }
        public bool Enabled => this.VCSettings.NotificationEnabled;
        #endregion Properties

        #region Methods
        public void HandleShow(DisplayTarget type, bool isSwitchEventType = true)
        {
            if (this.VCSettings.NotificationMode.Equals(type) && (isSwitchEventType || this.VCSettings.NotificationShowsVolumeChange))
                this.Show();
        }
        public new void Show()
        {
            if (!this.Enabled)
            {
                return;
            }

            base.Show();
            TimeoutTimer.StartOrReset();
        }
        public new void Hide()
        {
            if (_mouseOver)
            {
                TimeoutTimer.StartOrReset();
            }
            else
            {
                TimeoutTimer.Stop();
                base.Hide();
            }
        }
        #endregion Methods

        #region EventHandlers
        private void Handle_TimeoutTimerTick(object sender, EventArgs e) => this.Hide();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = true;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rect rect = SystemParameters.WorkArea;
            this.Left = rect.Right - this.ActualWidth - 10;
            this.Top = rect.Bottom - this.ActualHeight - 10;
        }
        private void ControlGotFocus(object sender, RoutedEventArgs e) => TimeoutTimer.Stop();

        private void ControlLostFocus(object sender, RoutedEventArgs e) => TimeoutTimer.Start();

        private void ControlMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseOver = true;
            TimeoutTimer.Stop();
        }
        private void ControlMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseOver = false;
            TimeoutTimer.Start();
        }
        private void ControlDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e) => TimeoutTimer.Stop();

        private void ControlDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) => TimeoutTimer.Start();

        private void ControlGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) => TimeoutTimer.Stop();

        private void ControlLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) => TimeoutTimer.Start();
        #endregion EventHandlers

        #region DependencyProperties
        #region SliderVisibility
        public static readonly DependencyProperty SliderVisibilityProperty = DependencyProperty.Register("SliderVisibility", typeof(Visibility), typeof(ListNotification), new(Visibility.Visible, OnSliderVisibilityChanged));
        public Visibility SliderVisibility
        {
            get => (Visibility)this.GetValue(SliderVisibilityProperty);
            set => this.SetValue(SliderVisibilityProperty, value);
        }
        private static void OnSliderVisibilityChanged(DependencyObject element, DependencyPropertyChangedEventArgs e) => ((ListNotification)element).SliderVisibility = (Visibility)e.NewValue;
        #endregion SliderVisibility

        #region SliderValue
        public static readonly DependencyProperty SliderValueProperty = DependencyProperty.Register("SliderValue", typeof(double), typeof(ListNotification), new(0.0, OnSliderValueChanged));
        public double SliderValue
        {
            get => slider.Value = Convert.ToDouble(this.GetValue(SliderValueProperty));
            set => this.SetValue(SliderValueProperty, slider.Value = value);
        }
        private static void OnSliderValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e) => ((ListNotification)element).SliderValue = (double)e.NewValue;
        #endregion SliderValue

        #region ValueText
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register("ValueText", typeof(string), typeof(ListNotification), new(string.Empty, OnValueTextChanged));
        public string? ValueText
        {
            get => Convert.ToString(this.GetValue(ValueTextProperty));
            set => this.SetValue(ValueTextProperty, value);
        }
        private static void OnValueTextChanged(DependencyObject element, DependencyPropertyChangedEventArgs e) => ((ListNotification)element).ValueText = Convert.ToString(e.NewValue);
        #endregion ValueText

        #region LockSelection
        public static readonly DependencyProperty LockSelectionProperty = DependencyProperty.Register("LockSelection", typeof(bool), typeof(ListNotification), new(false, OnLockSelectionChanged));
        public bool LockSelection
        {
            get => Convert.ToBoolean(this.GetValue(LockSelectionProperty));
            set => this.SetValue(LockSelectionProperty, value);
        }
        private static void OnLockSelectionChanged(DependencyObject element, DependencyPropertyChangedEventArgs e) => ((ListNotification)element).LockSelection = (bool)e.NewValue;
        #endregion LockSelection

        #region ShowIcons
        public static readonly DependencyProperty ShowIconsProperty = DependencyProperty.Register("ShowIcons", typeof(bool), typeof(ListNotification), new(Settings.ShowIcons, OnShowIconsChanged));
        public bool ShowIcons
        {
            get => Convert.ToBoolean(this.GetValue(ShowIconsProperty));
            set => this.SetValue(ShowIconsProperty, value);
        }
        private static void OnShowIconsChanged(DependencyObject element, DependencyPropertyChangedEventArgs e) => ((ListNotification)element).ShowIcons = Convert.ToBoolean(e.NewValue);
        #endregion ShowIcons
        #endregion DependencyProperties
    }
}
