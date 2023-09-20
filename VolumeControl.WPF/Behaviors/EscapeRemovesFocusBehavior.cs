using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// Removes logical and/or keyboard focus from the attached control when the Escape key is pressed.
    /// </summary>
    /// <remarks>
    /// By default, the logical and keyboard focus are set to the <see cref="Window"/> that owns the attached control.
    /// </remarks>
    public sealed class EscapeRemovesFocusBehavior : Behavior<Control>
    {
        #region Fields
        private bool _isAttached = false;
        #endregion Fields

        #region FocusOnProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="FocusOn"/>.
        /// </summary>
        public static readonly DependencyProperty FocusOnProperty = DependencyProperty.Register(
            nameof(FocusOn),
            typeof(Control),
            typeof(EscapeRemovesFocusBehavior),
            new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the <see cref="Control"/> that will be focused.
        /// </summary>
        /// <remarks>
        /// When this is set to a non-<see langword="null"/> value, the <see cref="FocusOnParentWindow"/> property is ignored.
        /// </remarks>
        public Control? FocusOn
        {
            get => (Control?)GetValue(FocusOnProperty);
            set => SetValue(FocusOnProperty, value);
        }
        #endregion FocusOnProperty

        #region FocusOnParentWindowProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="FocusOnParentWindow"/>.
        /// </summary>
        public static readonly DependencyProperty FocusOnParentWindowProperty = DependencyProperty.Register(
            nameof(FocusOnParentWindow),
            typeof(bool),
            typeof(EscapeRemovesFocusBehavior),
            new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether the focus will be set to the parent <see cref="Window"/> or cleared entirely.
        /// </summary>
        /// <remarks>
        /// This property is ignored when <see cref="FocusOn"/> is set to a non-<see langword="null"/> value.
        /// </remarks>
        public bool FocusOnParentWindow
        {
            get => (bool)GetValue(FocusOnParentWindowProperty);
            set => SetValue(FocusOnParentWindowProperty, value);
        }
        #endregion FocusOnParentWindowProperty

        #region AffectsKeyboardFocusProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="AffectsKeyboardFocus"/>.
        /// </summary>
        public static readonly DependencyProperty AffectsKeyboardFocusProperty = DependencyProperty.Register(
            nameof(AffectsKeyboardFocus),
            typeof(bool),
            typeof(EscapeRemovesFocusBehavior),
            new PropertyMetadata(true, OnAffectsKeyboardFocusPropertyChanged));
        /// <summary>
        /// Gets or sets whether the Keyboard focus is changed when the escape key is pressed.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when <see cref="Keyboard.FocusedElement"/> will be changed; otherwise <see langword="false"/>.
        /// </returns>
        public bool AffectsKeyboardFocus
        {
            get => (bool)GetValue(AffectsKeyboardFocusProperty);
            set => SetValue(AffectsKeyboardFocusProperty, value);
        }
        private static void OnAffectsKeyboardFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var inst = (EscapeRemovesFocusBehavior)d;
            var newState = (bool)e.NewValue;

            if (newState && !inst._isAttached)
            { // this property was enabled & event handler is not attached; attach it:
                inst.AttachEventHandler();
            }
            else if (!inst.AffectsLogicalFocus)
            { // this property was disabled & AffectsLogicalFocus is disabled; event handler method won't do anything, so detatch it:
                inst.DetatchEventHandler();
            }
        }
        #endregion AffectsKeyboardFocusProperty

        #region AffectsLogicalFocusProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="AffectsLogicalFocus"/>.
        /// </summary>
        public static readonly DependencyProperty AffectsLogicalFocusProperty = DependencyProperty.Register(
            nameof(AffectsLogicalFocus),
            typeof(bool),
            typeof(EscapeRemovesFocusBehavior),
            new PropertyMetadata(true, OnAffectsLogicalFocusPropertyChanged));
        /// <summary>
        /// Gets or sets whether the Logical focus is changed when the escape key is pressed.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when the <see cref="FocusManager.FocusedElementProperty"/> will be changed; otherwise <see langword="false"/>.
        /// </returns>
        public bool AffectsLogicalFocus
        {
            get => (bool)GetValue(AffectsLogicalFocusProperty);
            set => SetValue(AffectsLogicalFocusProperty, value);
        }
        private static void OnAffectsLogicalFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var inst = (EscapeRemovesFocusBehavior)d;
            var newState = (bool)e.NewValue;

            if (newState && !inst._isAttached)
            {
                inst.AttachEventHandler();
            }
            else if (!inst.AffectsKeyboardFocus)
            { // this property was disabled & AffectsKeyboardFocus is disabled; detatch event handler:
                inst.DetatchEventHandler();
            }
        }
        #endregion AffectsLogicalFocusProperty

        #region SetEventAsHandledProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="SetEventAsHandled"/>.
        /// </summary>
        public static readonly DependencyProperty SetEventAsHandledProperty = DependencyProperty.Register(
            nameof(SetEventAsHandled),
            typeof(bool),
            typeof(EscapeRemovesFocusBehavior),
            new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether PreviewKeyDown events for the Escape key are set as Handled.
        /// </summary>
        /// <remarks>
        /// PreviewKeyDown events where the <see cref="KeyEventArgs.Key"/> is not <see cref="Key.Escape"/> are never set as handled under any circumstances.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> when events are set as Handled; otherwise <see langword="false"/> when events are not set as handled.
        /// </returns>
        public bool SetEventAsHandled
        {
            get => (bool)GetValue(SetEventAsHandledProperty);
            set => SetValue(SetEventAsHandledProperty, value);
        }
        #endregion SetEventAsHandledProperty

        #region Methods
        /// <summary>
        /// Attaches the <see cref="AssociatedObject_PreviewKeyDown(object, KeyEventArgs)"/> handler method to the PreviewKeyDown event.
        /// </summary>
        private void AttachEventHandler()
        {
            if (_isAttached) return;

            this.AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            _isAttached = true;
        }
        /// <summary>
        /// Detatches the <see cref="AssociatedObject_PreviewKeyDown(object, KeyEventArgs)"/> handler method from the PreviewKeyDown event.
        /// </summary>
        private void DetatchEventHandler()
        {
            if (!_isAttached) return;

            this.AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
            _isAttached = false;
        }
        #endregion Methods

        #region EventHandlers
        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //< at this point, we know that AffectsKeyboardFocus and/or AffectsLogicalFocus is true
            switch (e.Key)
            {
            case Key.Escape:
                { // Escape key was pressed:
                    var focusScope = FocusManager.GetFocusScope(this.AssociatedObject);

                    if (FocusOn != null)
                    { // set focus to the specified element:
                        if (AffectsLogicalFocus)
                            FocusManager.SetFocusedElement(focusScope, FocusOn);
                        if (AffectsKeyboardFocus)
                            Keyboard.Focus(FocusOn);
                    }
                    else if (FocusOnParentWindow)
                    { // set focus to the parent window:
                        var window = Window.GetWindow(this.AssociatedObject);
                        if (AffectsLogicalFocus)
                            FocusManager.SetFocusedElement(focusScope, window);
                        if (AffectsKeyboardFocus)
                            Keyboard.Focus(window);
                    }
                    else
                    {
                        if (AffectsLogicalFocus)
                            FocusManager.SetFocusedElement(focusScope, null);
                        if (AffectsKeyboardFocus)
                            Keyboard.ClearFocus();
                    }

                    if (SetEventAsHandled)
                        e.Handled = true; //< ESC keypress has been handled
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
            this.AttachEventHandler();
            base.OnAttached();
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            this.DetatchEventHandler();
            base.OnDetaching();
        }
        #endregion Behavior Method Overrides
    }
}
