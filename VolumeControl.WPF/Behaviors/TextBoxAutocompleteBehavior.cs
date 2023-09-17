using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// Behavior that implements autocomplete suggestions for <see cref="TextBox"/> controls.
    /// </summary>
    public class TextBoxAutocompleteBehavior : Behavior<TextBox>
    {
        #region ItemsSourceProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable<string>),
            typeof(TextBoxAutocompleteBehavior),
            new PropertyMetadata(null, OnItemsSourcePropertyChanged));
        /// <summary>
        /// Gets or sets the source for auto complete suggestions.
        /// </summary>
        public IEnumerable<string> ItemsSource
        {
            get => (IEnumerable<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        /// <summary>
        /// Attaches event handlers when the ItemsSource is set to a valid value, and detaches event handlers when the ItemsSource is set to <see langword="null"/>.
        /// </summary>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autocomplete = (TextBoxAutocompleteBehavior)d;
            if (autocomplete.AssociatedObject is TextBox textBox)
            {
                textBox.TextChanged -= autocomplete.OnTextChanged;
                textBox.PreviewKeyDown -= autocomplete.OnPreviewKeyDown;

                if (e.NewValue != null)
                {
                    textBox.TextChanged += autocomplete.OnTextChanged;
                    textBox.PreviewKeyDown += autocomplete.OnPreviewKeyDown;
                }
            }
        }
        #endregion ItemsSourceProperty

        #region StringComparisonProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="StringComparison"/>.
        /// </summary>
        public static readonly DependencyProperty StringComparisonProperty = DependencyProperty.Register(
            nameof(StringComparison),
            typeof(StringComparison),
            typeof(TextBoxAutocompleteBehavior),
            new PropertyMetadata(StringComparison.Ordinal));
        /// <summary>
        /// Gets or sets the string comparison type to use when matching auto complete suggestions to the existing text.
        /// </summary>
        public StringComparison StringComparison
        {
            get => (StringComparison)GetValue(StringComparisonProperty);
            set => SetValue(StringComparisonProperty, value);
        }
        #endregion StringComparisonProperty

        #region RequirePrefixProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="RequirePrefix"/>.
        /// </summary>
        public static readonly DependencyProperty RequirePrefixProperty = DependencyProperty.Register(
            nameof(RequirePrefix),
            typeof(string),
            typeof(TextBoxAutocompleteBehavior),
            new PropertyMetadata(string.Empty));
        /// <summary>
        /// Gets or sets the string prefix that must appear at the start of the text for autocomplete suggestions to appear.
        /// </summary>
        public string RequirePrefix
        {
            get => (string)GetValue(RequirePrefixProperty);
            set => SetValue(RequirePrefixProperty, value);
        }
        #endregion RequirePrefixProperty

        #region Methods
        /// <remarks>
        /// Code is based on <see href="https://www.nuget.org/packages/WPFTextBoxAutoComplete">Nimgoble/WPFTextBoxAutoComplete</see>.
        /// </remarks>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is null) return;

            if (e.Changes.Where((change) => change.RemovedLength > 0).Any() && !e.Changes.Where((change) => change.AddedLength > 0).Any())
                return;

            if (ItemsSource == null || string.IsNullOrEmpty(AssociatedObject.Text))
                return;

            int num = 0;
            string matchingString = AssociatedObject.Text;
            if (!string.IsNullOrEmpty(RequirePrefix))
            {
                num = AssociatedObject.Text.LastIndexOf(RequirePrefix);
                if (num == -1)
                    return;

                num += RequirePrefix.Length;
                matchingString = AssociatedObject.Text[num..];
            }

            if (!string.IsNullOrEmpty(matchingString))
            {
                int textLength = matchingString.Length;
                StringComparison comparer = StringComparison;
                string? text = (from subvalue in ItemsSource
                                where subvalue != null && subvalue.Length >= textLength
                                select subvalue into value
                                where value[..textLength].Equals(matchingString, comparer)
                                select value[textLength..]).FirstOrDefault();
                if (!string.IsNullOrEmpty(text))
                {
                    int num2 = num + matchingString.Length;
                    AssociatedObject.TextChanged -= OnTextChanged;
                    AssociatedObject.Text += text;
                    AssociatedObject.CaretIndex = num2;
                    AssociatedObject.SelectionStart = num2;
                    AssociatedObject.SelectionLength = AssociatedObject.Text.Length - num;
                    AssociatedObject.TextChanged += OnTextChanged;
                }
            }
        }
        /// <remarks>
        /// Code is based on <see href="https://www.nuget.org/packages/WPFTextBoxAutoComplete">Nimgoble/WPFTextBoxAutoComplete</see>.
        /// </remarks>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (AssociatedObject != null && AssociatedObject.SelectionLength > 0 && AssociatedObject.SelectionStart + AssociatedObject.SelectionLength == AssociatedObject.Text.Length)
                {
                    AssociatedObject.SelectionStart = AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
                    AssociatedObject.SelectionLength = 0;
                }
            }
        }
        #endregion Methods

        #region Behavior Override Methods
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TextChanged += OnTextChanged;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.TextChanged -= OnTextChanged;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        }
        #endregion Behavior Override Methods
    }
}
