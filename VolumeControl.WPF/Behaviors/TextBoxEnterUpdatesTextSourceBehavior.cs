using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// Forces the <see cref="TextBox.Text"/> property's data binding to update its source when the Enter key is pressed or released.
    /// </summary>
    public sealed class TextBoxEnterUpdatesTextSourceBehavior : Behavior<TextBoxBase>
    {
        #region Fields
        private bool _isAttached = false;
        #endregion Fields

        #region OnKeyEventProperty
        /// <summary>
        /// Defines the key press events that the <see cref="TextBoxEnterUpdatesTextSourceBehavior"/> can attach to.
        /// </summary>
        public enum EKeyEvent
        {
            /// <summary>
            /// The Text databinding is updated when the Enter key is released.
            /// </summary>
            KeyUp,
            /// <summary>
            /// The Text databinding is updated when the Enter key is pressed.
            /// </summary>
            KeyDown,
        }
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="OnKeyEvent"/>.
        /// </summary>
        public static readonly DependencyProperty OnKeyEventProperty = DependencyProperty.Register(
            nameof(OnKeyEvent),
            typeof(EKeyEvent),
            typeof(TextBoxEnterUpdatesTextSourceBehavior),
            new PropertyMetadata(EKeyEvent.KeyUp, OnKeyEventPropertyChanged));
        /// <summary>
        /// Gets or sets whether to update the Text binding's source when the Enter key is pressed or when it is released.
        /// </summary>
        /// <returns>
        /// <see cref="EKeyEvent.KeyUp"/> when updates occur when released; <see cref="EKeyEvent.KeyDown"/> when updates occur when pressed.
        /// </returns>
        public EKeyEvent OnKeyEvent
        {
            get => (EKeyEvent)GetValue(OnKeyEventProperty);
            set => SetValue(OnKeyEventProperty, value);
        }
        private static void OnKeyEventPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var inst = (TextBoxEnterUpdatesTextSourceBehavior)d;

            inst.DetatchEventHandler((EKeyEvent)e.OldValue);
            inst.AttachEventHandler((EKeyEvent)e.NewValue);
        }
        #endregion OnKeyEventProperty

        #region Methods
        private void AttachEventHandler(EKeyEvent keyEvent)
        {
            if (_isAttached) return;

            switch (keyEvent)
            {
            case EKeyEvent.KeyUp:
                this.AssociatedObject.KeyUp += AssociatedObject_KeyEvent;
                break;
            case EKeyEvent.KeyDown:
                this.AssociatedObject.KeyDown += AssociatedObject_KeyEvent;
                break;
            }
            _isAttached = true;
        }
        private void DetatchEventHandler(EKeyEvent keyEvent)
        {
            if (!_isAttached) return;

            switch (keyEvent)
            {
            case EKeyEvent.KeyUp:
                this.AssociatedObject.KeyUp -= AssociatedObject_KeyEvent;
                break;
            case EKeyEvent.KeyDown:
                this.AssociatedObject.KeyDown -= AssociatedObject_KeyEvent;
                break;
            }
            _isAttached = false;
        }
        #endregion Methods

        #region EventHandlers
        private void AssociatedObject_KeyEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
            case Key.Enter:
                { // Enter key was pressed:
                    // GetBindingExpression returns null if the property isn't bound
                    if (BindingOperations.GetBindingExpression(this.AssociatedObject, TextBox.TextProperty) is BindingExpression expr)
                    {
                        expr.UpdateSource(); //< UpdateSource does nothing if the binding mode doesn't allow updating the source
                    }
                    break;
                }
            default: break;
            }
        }
        #endregion EventHandlers

        #region Behavior Method Overrides
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            this.AttachEventHandler(OnKeyEvent);
            base.OnAttached();
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            this.DetatchEventHandler(OnKeyEvent);
            base.OnDetaching();
        }
        #endregion Behavior Method Overrides
    }
}
