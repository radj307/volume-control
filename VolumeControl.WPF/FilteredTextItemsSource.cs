using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Helper object for creating autocomplete suggestion dropdowns for textboxes.
    /// </summary>
    public class FilteredTextItemsSource : DependencyObject
    {
        #region ItemsSourceProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable<string>),
            typeof(FilteredTextItemsSource),
            new PropertyMetadata(null, OnItemsSourcePropertyChanged));
        /// <summary>
        /// Gets or sets the source list.
        /// </summary>
        public IEnumerable<string> ItemsSource
        {
            get => (IEnumerable<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var inst = (FilteredTextItemsSource)d;

            inst.UpdateFilteredItemsSource();
        }
        #endregion ItemsSourceProperty

        #region FilterTextProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="FilterText"/>.
        /// </summary>
        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(
            nameof(FilterText),
            typeof(string),
            typeof(FilteredTextItemsSource),
            new PropertyMetadata(string.Empty, OnFilterTextPropertyChanged));
        /// <summary>
        /// Gets or sets the string that determines which items are present in the FilteredItemsSource.
        /// </summary>
        /// <remarks>
        /// <see cref="FilteredItemsSource"/> is repopulated when this property is changed.
        /// </remarks>
        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }
        private static void OnFilterTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var inst = (FilteredTextItemsSource)d;

            inst.UpdateFilteredItemsSource();
        }
        #endregion FilterTextProperty

        #region FilteredItemsSourceProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="FilteredItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty FilteredItemsSourceProperty = DependencyProperty.Register(
            nameof(FilteredItemsSource),
            typeof(IEnumerable<string>),
            typeof(FilteredTextItemsSource),
            new PropertyMetadata(null));
        /// <summary>
        /// Gets the filtered list.
        /// </summary>
        /// <remarks>
        /// Bind the ItemsSource of your options list to this property.
        /// </remarks>
        public IEnumerable<string>? FilteredItemsSource
        {
            get => (IEnumerable<string>?)GetValue(FilteredItemsSourceProperty);
            protected set => SetValue(FilteredItemsSourceProperty, value);
        }
        #endregion FilteredItemsSourceProperty

        #region StringComparisonProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="StringComparison"/>.
        /// </summary>
        public static readonly DependencyProperty StringComparisonProperty = DependencyProperty.Register(
            nameof(StringComparison),
            typeof(StringComparison),
            typeof(FilteredTextItemsSource),
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

        #region Methods
        /// <summary>
        /// Gets the filtered items source enumeration from the current ItemsSource &amp; FilterText.
        /// </summary>
        /// <returns>All of the items that start with the current FilterText.</returns>
        protected virtual IEnumerable<string>? GetFilteredItemsSource()
        {
            if (ItemsSource != null && !string.IsNullOrEmpty(FilterText))
            {
                int textLength = FilterText.Length;
                StringComparison comparer = StringComparison;

                return from subvalue in ItemsSource
                       where subvalue != null && subvalue.Length > textLength
                       select subvalue into value
                       where value[..textLength].Equals(FilterText, comparer)
                       select value;
            }

            return ItemsSource;
        }
        /// <summary>
        /// Updates the <see cref="FilteredItemsSource"/> property using <see cref="GetFilteredItemsSource"/>.
        /// </summary>
        private void UpdateFilteredItemsSource()
            => FilteredItemsSource = GetFilteredItemsSource();
        #endregion Methods
    }
}
