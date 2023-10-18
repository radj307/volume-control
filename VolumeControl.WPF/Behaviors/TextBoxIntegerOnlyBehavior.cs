using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// TextBox behavior that uses regular expressions to only allow certain types of input.
    /// </summary>
    public sealed class TextBoxInputFilterBehavior : Behavior<TextBoxBase>
    {
        #region Behavior Method Overrides
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            AssociatedObject.PreviewTextInput += this.AssociatedObject_PreviewTextInput;
            base.OnAttached();
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewTextInput -= this.AssociatedObject_PreviewTextInput;
            base.OnDetaching();
        }
        #endregion Behavior Method Overrides

        #region Properties
        /// <summary>
        /// Gets or sets the regular expression used to determine whether a text input is valid or not.
        /// </summary>
        /// <remarks>
        /// If the text input does NOT match the regular expression, it is not allowed.
        /// </remarks>
        public string? Regex
        {
            get => _regex?.ToString();
            set
            {
                _regex = value == null ? null : new Regex(value, RegexOptions.Compiled);
            }
        }
        private Regex? _regex;
        #endregion Properties

        #region EventHandlers
        private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (_regex == null) return;
            if (!_regex.IsMatch(e.Text))
            { // text is NOT a number:
                e.Handled = true;
            }
        }
        #endregion EventHandlers
    }
}
