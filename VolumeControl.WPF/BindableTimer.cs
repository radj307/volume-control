using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using VolumeControl.WPF.Extensions;

namespace VolumeControl.WPF
{
    /// <inheritdoc cref="System.Timers.Timer"/>
    /// <remarks>
    /// Uses <see cref="System.Timers.Timer"/> &amp; supports WPF data binding for related properties.
    /// </remarks>
    [DefaultProperty(nameof(Interval))]
    [DefaultEvent(nameof(Elapsed))]
    public sealed class BindableTimer : DependencyObject, ISupportInitialize, IComponent, IDisposable
    {
        #region Initializer
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableTimer"/> class, and sets all properties to their default values.
        /// </summary>
        public BindableTimer()
        {
            // initialize timer & set its properties to match the default ones specified in this class' DependencyProperties
            Timer = new()
            {
                AutoReset = this.AutoReset,
                Enabled = this.Enabled,
                Interval = this.Interval,
            };
            Timer.Elapsed += NotifyElapsed;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref='BindableTimer'/> class, setting the <see cref='Interval'/> property to the period specified by <paramref name="interval"/>.
        /// </summary>
        /// <param name="interval">The time, in milliseconds, between events. The value must be greater than zero and less than or equal to <see cref="int.MaxValue"/>.</param>
        public BindableTimer(double interval) : this()
        {
            Interval = interval;
        }
        #endregion Initializer

        #region Fields
        private bool disposedValue;
        private readonly System.Timers.Timer Timer;
        /// <summary>
        /// Gets the default <see cref="DefaultEnabled"/> value.
        /// </summary>
        public const bool DefaultEnabled = false;
        /// <summary>
        /// Gets the default <see cref="Interval"/> value.
        /// </summary>
        public const double DefaultInterval = 100.0;
        /// <summary>
        /// Gets the default <see cref="MinInterval"/> value.
        /// </summary>
        public const double DefaultMinInterval = 1.0;
        /// <summary>
        /// Gets the default <see cref="DefaultMaxInterval"/> value.
        /// </summary>
        public const double DefaultMaxInterval = int.MaxValue;
        /// <summary>
        /// Gets the default <see cref="AutoReset"/> value.
        /// </summary>
        public const bool DefaultAutoReset = true;
        #endregion Fields

        #region Events
        /// <inheritdoc cref="System.Timers.Timer.Elapsed"/>
        public event System.Timers.ElapsedEventHandler? Elapsed;
        private void NotifyElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // disable the timer
            Dispatcher.InvokeIfRequired(() =>
            {
                if (!AutoReset) Enabled = false;
            });
            // invoke the elapsed event (discard the original sender, which is private)
            Elapsed?.Invoke(this, e);
        }
        #endregion Events

        #region EnabledProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> instance for the <see cref="Enabled"/> property.
        /// </summary>
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
            nameof(Enabled),
            typeof(bool),
            typeof(BindableTimer),
            new PropertyMetadata(DefaultEnabled, OnEnabledPropertyChanged));
        /// <inheritdoc cref="System.Timers.Timer.Enabled"/>
        public bool Enabled
        {
            get => (bool)GetValue(EnabledProperty);
            set => SetValue(EnabledProperty, value);
        }
        private static void OnEnabledPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            BindableTimer bindableTimer = (BindableTimer)element;

            bindableTimer.Timer.Enabled = (bool)e.NewValue;
        }
        #endregion EnabledProperty

        #region IntervalProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> instance for the <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            nameof(Interval),
            typeof(double),
            typeof(BindableTimer),
            new PropertyMetadata(DefaultInterval, OnIntervalPropertyChanged, OnIntervalCoerceValue));
        /// <inheritdoc cref="System.Timers.Timer.Interval"/>
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }
        private static void OnIntervalPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            BindableTimer bindableTimer = (BindableTimer)element;

            bindableTimer.Timer.Interval = (double)e.NewValue;
        }
        private static object OnIntervalCoerceValue(DependencyObject element, object baseValue)
        {
            BindableTimer bindableTimer = (BindableTimer)element;

            double value = (double)baseValue;

            // prevent exceptions from going below minimum interval allowed by System.Timers.Timer
            if (value <= bindableTimer.MinInterval)
                value = bindableTimer.MinInterval;
            // prevent exceptions from exceeding maximum interval allowed by System.Timers.Timer
            else if (value > int.MaxValue)
                value = int.MaxValue;

            return value;
        }
        #endregion IntervalProperty

        #region MinIntervalProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> instance for the <see cref="MinInterval"/> property.
        /// </summary>
        public static readonly DependencyProperty MinIntervalProperty = DependencyProperty.Register(
            nameof(MinInterval),
            typeof(double),
            typeof(BindableTimer),
            new PropertyMetadata(DefaultMinInterval));
        /// <summary>
        /// Gets or sets the minimum interval, expressed in milliseconds, at which to raise the <see cref="Elapsed"/> event.<br/>
        /// When attempting to set the interval to a value less than this number, the interval is set to this number instead.
        /// </summary>
        /// <remarks>
        /// The minimum possible interval allowed by <see cref="System.Timers.Timer"/> cannot be less than or equal to zero.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value is less than or equal to zero.</exception>
        public double MinInterval
        {
            get => (double)GetValue(MinIntervalProperty);
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException(nameof(MinInterval), value, "Minimum timer interval must be greater than 0.0!");
                SetValue(MinIntervalProperty, value);
            }
        }
        #endregion MinIntervalProperty

        #region MaxIntervalProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> instance for the <see cref="MaxInterval"/> property.
        /// </summary>
        public static readonly DependencyProperty MaxIntervalProperty = DependencyProperty.Register(
            nameof(MaxInterval),
            typeof(double),
            typeof(BindableTimer),
            new PropertyMetadata(DefaultMaxInterval));
        /// <summary>
        /// Gets or sets the maximum interval, expressed in milliseconds, at which to raise the <see cref="Elapsed"/> event.<br/>
        /// When attempting to set the interval to a value greater than this number, the interval is set to this number instead.
        /// </summary>
        /// <remarks>
        /// The maximum possible interval allowed by <see cref="System.Timers.Timer"/> cannot be greater than <see cref="int.MaxValue"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Value is greater than <see cref="int.MaxValue"/>.</exception>
        public double MaxInterval
        {
            get => (double)GetValue(MaxIntervalProperty);
            set
            {
                if (value > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(MaxInterval), value, $"Maximum timer interval must be less than {nameof(int.MaxValue)} ({int.MaxValue})!");
                SetValue(MaxIntervalProperty, value);
            }
        }
        #endregion MaxIntervalProperty

        #region AutoResetProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> instance for the <see cref="AutoReset"/> property.
        /// </summary>
        public static readonly DependencyProperty AutoResetProperty = DependencyProperty.Register(
            nameof(AutoReset),
            typeof(bool),
            typeof(BindableTimer),
            new PropertyMetadata(DefaultAutoReset, OnAutoResetPropertyChanged));
        /// <inheritdoc cref="System.Timers.Timer.AutoReset"/>
        public bool AutoReset
        {
            get => (bool)GetValue(AutoResetProperty);
            set => SetValue(AutoResetProperty, value);
        }
        private static void OnAutoResetPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            BindableTimer bindableTimer = (BindableTimer)element;

            bindableTimer.Timer.AutoReset = (bool)e.NewValue;
        }
        #endregion AutoResetProperty

        #region Methods
        /// <inheritdoc cref="System.Timers.Timer.Close"/>
        public void Close()
        {// do not edit:
            this.Dispose(); //< let 
        }
        /// <inheritdoc cref="System.Timers.Timer.Start"/>
        public void Start()
        {// do not edit:
            Enabled = true; //< let the Enabled property change the timer's state so we update bindings
        }
        /// <inheritdoc cref="System.Timers.Timer.Stop"/>
        public void Stop()
        {// do not edit:
            Enabled = false; //< let the Enabled property change the timer's state so we update bindings
        }
        /// <summary>
        /// Resets the elapsed time without triggering the <see cref="Elapsed"/> event. If the timer is stopped, it will be started.
        /// </summary>
        /// <remarks>
        /// Does not cause the value of the <see cref="Enabled"/> property to change, unless the timer was stopped prior to calling this method.
        /// </remarks>
        public void Restart()
        {// do not edit:
            Timer.Enabled = false; //< set the timer's Enabled property directly so we don't cause the local Enabled property to update
            Enabled = true; //< set the local Enabled property so it updates only when the timer was previously disabled
        }
        /// <inheritdoc cref="BindingOperations.SetBinding(DependencyObject, DependencyProperty, BindingBase)"/>
        public BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding) => BindingOperations.SetBinding(this, dp, binding);
        #endregion Methods

        #region ISupportInitialize Implementation
        /// <inheritdoc/>
        public void BeginInit()
            => ((ISupportInitialize)Timer).BeginInit();
        /// <inheritdoc/>
        public void EndInit()
            => ((ISupportInitialize)Timer).EndInit();
        #endregion ISupportInitialize Implementation

        #region IComponent Implementation
        /// <inheritdoc/>
        public ISite? Site { get => ((IComponent)Timer).Site; set => ((IComponent)Timer).Site = value; }
        /// <inheritdoc/>
        public event EventHandler? Disposed
        {
            add => ((IComponent)Timer).Disposed += value;
            remove => ((IComponent)Timer).Disposed -= value;
        }
        #endregion IComponent Implementation

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of the <see cref="BindableTimer"/> instance and the underlying timer object.
        /// </summary>
        ~BindableTimer() => Dispose(disposing: true);
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Enabled = false; //< set this to false before disposing of the timer, or things will break

                if (disposing)
                {
                    Timer.Dispose();
                }

                disposedValue = true;
            }
        }
        /// <summary>
        /// Disposes of the <see cref="BindableTimer"/> instance and the underlying timer object.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
