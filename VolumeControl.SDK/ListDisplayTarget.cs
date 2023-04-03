using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VolumeControl.Core.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.SDK
{
    /// <summary>
    /// Wrapper object for a ListNotification display target.<br/>
    /// This provides the necessary interaction interfaces for the ListNotification window to display a list of items.
    /// </summary>
    public sealed class ListDisplayTarget : DependencyObject, IListDisplayTarget, INotifyPropertyChanged
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance.
        /// </summary>
        /// <param name="conditionalShowTriggers">Any number of <see cref="ConditionalEventForward"/> instances that can cause the ListNotification window to become visible.</param>
        public ListDisplayTarget(params ConditionalEventForward[] conditionalShowTriggers)
        {
            _conditionalShowTriggers = conditionalShowTriggers;
            this.HookUpConditionalShowTriggers();

            this.Selected += (s, e) => IsSelected = true;
            this.Unselected += (s, e) => IsSelected = false;
        }
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance.
        /// </summary>
        /// <param name="name">A name to assign to this <see cref="ListDisplayTarget"/> instance. This is shown in the DisplayTarget selector.</param>
        /// <param name="conditionalShowTriggers">Any number of <see cref="ConditionalEventForward"/> instances that can cause the ListNotification window to become visible.</param>
        public ListDisplayTarget(string name, params ConditionalEventForward[] conditionalShowTriggers) : this(conditionalShowTriggers) => this.Name = name;
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance.
        /// </summary>
        /// <param name="name">A name to assign to this <see cref="ListDisplayTarget"/> instance. This is shown in the DisplayTarget selector.</param>
        public ListDisplayTarget(string name = "") : this(name, Array.Empty<ConditionalEventForward>()) { }
        #endregion Constructors

        #region IsSelected
        /// <summary>
        /// Gets whether this <see cref="ListDisplayTarget"/> instance is currently selected.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when selected; otherwise <see langword="false"/>.
        /// </returns>
        public bool IsSelected { get; private set; }
        #endregion IsSelected

        #region Name
        /// <summary>
        /// The name of this display target.
        /// </summary>
        public string Name { get; set; } = nameof(ListDisplayTarget);
        #endregion Name

        #region BackgroundProperty
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Background"/> brush.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ListDisplayTarget), new PropertyMetadata(null));
        /// <inheritdoc/>
        public Brush? Background
        {
            get => this.GetValue(BackgroundProperty) as Brush;
            set => this.SetValue(BackgroundProperty, value);
        }
        #endregion BackgroundProperty

        #region LockSelectionProperty
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="LockSelection"/> state.
        /// </summary>
        public static readonly DependencyProperty LockSelectionProperty = DependencyProperty.Register(nameof(LockSelection), typeof(bool), typeof(ListDisplayTarget), new PropertyMetadata(null));
        /// <inheritdoc/>
        public bool LockSelection
        {
            get => (bool)this.GetValue(LockSelectionProperty);
            set => this.SetValue(LockSelectionProperty, value);
        }
        #endregion IconProperty

        #region IconProperty
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Icon"/> icon.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(ListDisplayTarget), new PropertyMetadata(null));
        /// <inheritdoc/>
        public ImageSource? Icon
        {
            get => this.GetValue(IconProperty) as ImageSource;
            set => this.SetValue(IconProperty, value);
        }
        #endregion IconProperty

        #region ItemsSourceProperty
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="ItemsSource"/> list source.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<IListDisplayable>), typeof(ListDisplayTarget), new PropertyMetadata(null, HandleItemsSourcePropertyChanged));
        private static void HandleItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ListDisplayTarget)?.UnsetSelectedItem();
        private void UnsetSelectedItem() => this.SelectedItem = null;
        /// <inheritdoc/>
        public IEnumerable<IListDisplayable>? ItemsSource
        {
            get => this.GetValue(ItemsSourceProperty) as IEnumerable<IListDisplayable>;
            set => this.SetValue(ItemsSourceProperty, value);
        }
        #endregion ItemsSourceProperty

        #region SelectedItemProperty
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="SelectedItem"/> item.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(IListDisplayable), typeof(ListDisplayTarget), new PropertyMetadata(null, HandleSelectedItemPropertyChanged));
        private static void HandleSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listDisplayTarget = (ListDisplayTarget)d;
            listDisplayTarget.NotifySelectedItemChanged();
            listDisplayTarget.PropertyChanged?.Invoke(listDisplayTarget, new(nameof(SelectedItemControls)));
        }
        /// <inheritdoc/>
        public IListDisplayable? SelectedItem
        {
            get => this.GetValue(SelectedItemProperty) as IListDisplayable;
            set => this.SetValue(SelectedItemProperty, value);
        }
        #endregion SelectedItemProperty

        #region SelectedItemControlsProperty
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="SelectedItemControls"/> control array.
        /// </summary>
        public static readonly DependencyProperty SelectedItemControlsProperty = DependencyProperty.Register(nameof(SelectedItemControls), typeof(Control[]), typeof(ListDisplayTarget), new PropertyMetadata(null));
        /// <inheritdoc/>
        public Control[]? SelectedItemControls
        {
            get => this.GetValue(SelectedItemControlsProperty) as Control[];
            set => this.SetValue(SelectedItemControlsProperty, value);
        }
        #endregion SelectedItemControlsProperty

        #region ConditionalShowTriggers
        /// <summary>
        /// A condition delegate that does not accept parameters &amp; returns <see cref="bool"/>.
        /// </summary>
        /// <returns><see langword="true"/> causes the ListNotification window to be shown; <see langword="false"/> prevents the window from being shown.</returns>
        public delegate bool ListDisplayConditional();
        private ConditionalEventForward[] _conditionalShowTriggers;
        /// <summary>
        /// An array of <see cref="ConditionalEventForward"/> instances that are automatically attached to this object via this property's setter method.
        /// </summary>
        public ConditionalEventForward[] ConditionalShowTriggers
        {
            get => _conditionalShowTriggers;
            set
            {
                this.UnHookConditionalShowTriggers();
                _conditionalShowTriggers = value;
                this.HookUpConditionalShowTriggers();
            }
        }
        private void HookUpConditionalShowTriggers()
            => this.ConditionalShowTriggers.ForEach(t => t.Event += this.NotifyShowEvent);
        private void UnHookConditionalShowTriggers()
            => this.ConditionalShowTriggers.ForEach(t => t.Event -= this.NotifyShowEvent);
        #endregion ConditionalShowTriggers

        #region Events
#   pragma warning disable CS0067 // The event 'ListNotificationVM.PropertyChanged' is never used.
        // We're using Fody for this, so disable unused warnings:
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#   pragma warning restore CS0067 // The event 'ListNotificationVM.PropertyChanged' is never used.
        /// <summary>
        /// Triggered when the <see cref="SelectedItem"/> is changed.
        /// </summary>
        public event EventHandler<IListDisplayable?>? SelectedItemChanged;
        private void NotifySelectedItemChanged() => SelectedItemChanged?.Invoke(this, this.SelectedItem);
        /// <summary>
        /// Triggered when any one of this <see cref="ListDisplayTarget"/> instance's <see cref="ConditionalShowTriggers"/> fire.
        /// </summary>
        public event EventHandler<object>? ShowEvent;
        private void NotifyShowEvent(object? sender, object e) => ShowEvent?.Invoke(sender, e);
        /// <summary>
        /// Triggers the <see cref="ShowEvent"/>, causing the notification to show the display target, if it's enabled.
        /// </summary>
        public void Show() => ShowEvent?.Invoke(this, new());
        /// <summary>
        /// Triggered when this <see cref="ListDisplayTarget"/> instance becomes the active display target.
        /// </summary>
        public event EventHandler? Selected; //< triggered via reflection
        /// <summary>
        /// Triggered when this <see cref="ListDisplayTarget"/> instance is no longer the active display target.
        /// </summary>
        public event EventHandler? Unselected; //< triggered via reflection
        #endregion Events

        #region Methods
        /// <inheritdoc cref="BindingOperations.SetBinding(DependencyObject, DependencyProperty, BindingBase)"/>
        public BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding) => BindingOperations.SetBinding(this, dp, binding);
        /// <summary>
        /// Creates a new <see cref="ConditionalEventForward"/> instance, attaches it to this <see cref="ListDisplayTarget"/> instance, then returns the instance so you can attach its handler to events.
        /// </summary>
        /// <param name="evaluator">An evaluator delegate method that determines whether an event will be forwarded or not.</param>
        /// <returns>A new <see cref="ConditionalEventForward"/> instance bound to this <see cref="ListDisplayTarget"/>.</returns>
        public ConditionalEventForward AddConditionalEventForward(ConditionalEventForward.ConditionEvaluator evaluator)
        {
            var inst = new ConditionalEventForward(evaluator);
            this.ConditionalShowTriggers = this.ConditionalShowTriggers.Append(inst).ToArray();
            return inst;
        }
        #endregion Methods
    }
}
