using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Interfaces;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// Wrapper object for a ListNotification display target.<br/>
    /// This provides the necessary interaction interfaces for the ListNotification window to display a list of items.
    /// </summary>
    public sealed class ListDisplayTarget : DependencyObject, INotifyPropertyChanged
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance.
        /// </summary>
        /// <param name="conditionalShowTriggers">Any number of <see cref="ConditionalEventForward"/> instances that can cause the <see cref="ListNotification"/> window to become visible.</param>
        public ListDisplayTarget(params ConditionalEventForward[] conditionalShowTriggers)
        {
            _conditionalShowTriggers = conditionalShowTriggers;
            HookUpConditionalShowTriggers();
        }
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance.
        /// </summary>
        /// <param name="name">A name to assign to this <see cref="ListDisplayTarget"/> instance. This is shown in the DisplayTarget selector.</param>
        /// <param name="conditionalShowTriggers">Any number of <see cref="ConditionalEventForward"/> instances that can cause the <see cref="ListNotification"/> window to become visible.</param>
        public ListDisplayTarget(string name, params ConditionalEventForward[] conditionalShowTriggers) : this(conditionalShowTriggers) => Name = name;
        /// <summary>
        /// Creates a new <see cref="ListDisplayTarget"/> instance.
        /// </summary>
        /// <param name="name">A name to assign to this <see cref="ListDisplayTarget"/> instance. This is shown in the DisplayTarget selector.</param>
        public ListDisplayTarget(string name = "") : this(name, Array.Empty<ConditionalEventForward>()) { }
        #endregion Constructors

        #region Name
        private string _name = nameof(ListDisplayTarget);
        /// <summary>
        /// The name of this display target.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        #endregion Name

        #region BackgroundProperty
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ListDisplayTarget), new PropertyMetadata(null));
        public Brush? Background
        {
            get => this.GetValue(BackgroundProperty) as Brush;
            set => this.SetValue(BackgroundProperty, value);
        }
        #endregion BackgroundProperty

        #region LockSelectionProperty
        public static readonly DependencyProperty LockSelectionProperty = DependencyProperty.Register(nameof(LockSelection), typeof(bool), typeof(ListDisplayTarget), new PropertyMetadata(null));
        public bool LockSelection
        {
            get => (bool)this.GetValue(LockSelectionProperty);
            set => this.SetValue(LockSelectionProperty, value);
        }
        #endregion IconProperty

        #region IconProperty
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(ListDisplayTarget), new PropertyMetadata(null));
        public ImageSource? Icon
        {
            get => this.GetValue(IconProperty) as ImageSource;
            set => this.SetValue(IconProperty, value);
        }
        #endregion IconProperty

        #region ItemsSourceProperty
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<IListDisplayable>), typeof(ListDisplayTarget), new PropertyMetadata(null, HandleItemsSourcePropertyChanged));
        private static void HandleItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ListDisplayTarget)?.UnsetSelectedItem();
        private void UnsetSelectedItem() => SelectedItem = null;
        public IEnumerable<IListDisplayable>? ItemsSource
        {
            get => this.GetValue(ItemsSourceProperty) as IEnumerable<IListDisplayable>;
            set => this.SetValue(ItemsSourceProperty, value);
        }
        #endregion ItemsSourceProperty

        #region SelectedItemProperty
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(IListDisplayable), typeof(ListDisplayTarget), new PropertyMetadata(null, HandleSelectedItemPropertyChanged));
        private static void HandleSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ListDisplayTarget)?.NotifySelectedItemChanged();
        public IListDisplayable? SelectedItem
        {
            get => this.GetValue(SelectedItemProperty) as IListDisplayable;
            set => this.SetValue(SelectedItemProperty, value);
        }
        #endregion SelectedItemProperty

        #region ConditionalShowTriggers
        /// <summary>
        /// A condition delegate that does not accept parameters &amp; returns <see cref="bool"/>.
        /// </summary>
        /// <returns><see langword="true"/> causes the <see cref="ListNotification"/> window to be shown; <see langword="false"/> prevents the window from being shown.</returns>
        public delegate bool ListDisplayConditional();
        private ConditionalEventForward[] _conditionalShowTriggers;
        public ConditionalEventForward[] ConditionalShowTriggers
        {
            get => _conditionalShowTriggers;
            set
            {
                UnHookConditionalShowTriggers();
                _conditionalShowTriggers = value;
                HookUpConditionalShowTriggers();
            }
        }
        private void HookUpConditionalShowTriggers() => ConditionalShowTriggers.ForEach(t => t.Event += NotifyShowEvent);
        private void UnHookConditionalShowTriggers() => ConditionalShowTriggers.ForEach(t => t.Event -= NotifyShowEvent);
        #endregion ConditionalShowTriggers

        #region Events
#   pragma warning disable CS0067 // The event 'ListNotificationVM.PropertyChanged' is never used.
        // We're using Fody for this, so disable unused warnings:
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#   pragma warning restore CS0067 // The event 'ListNotificationVM.PropertyChanged' is never used.
        public event EventHandler<IListDisplayable?>? SelectedItemChanged;
        private void NotifySelectedItemChanged() => SelectedItemChanged?.Invoke(this, SelectedItem);
        internal event EventHandler<object>? ShowEvent;
        private void NotifyShowEvent(object? sender, object e) => ShowEvent?.Invoke(sender, e);
        /// <summary>
        /// Triggered when this <see cref="ListDisplayTarget"/> instance is selected by the <see cref="ListNotificationVM"/>.
        /// </summary>
        public event EventHandler? Selected;
        internal void NotifySelected() => Selected?.Invoke(this, EventArgs.Empty);
        /// <summary>
        /// Triggered when this <see cref="ListDisplayTarget"/> instance is unselected by the <see cref="ListNotificationVM"/>.
        /// </summary>
        public event EventHandler? Unselected;
        internal void NotifyUnselected() => Unselected?.Invoke(this, EventArgs.Empty);
        #endregion Events

        #region Methods
        /// <inheritdoc cref="BindingOperations.SetBinding(DependencyObject, DependencyProperty, BindingBase)"/>
        public BindingExpressionBase SetBinding(DependencyProperty dp, BindingBase binding) => BindingOperations.SetBinding(this, dp, binding);

        public ConditionalEventForward AddConditionalEventForward(ConditionalEventForward.ConditionEvaluator evaluator)
        {
            var inst = new ConditionalEventForward(evaluator);
            ConditionalShowTriggers = ConditionalShowTriggers.Append(inst).ToArray();
            return inst;
        }
        #endregion Methods
    }
}
