using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace VolumeControl.WPF.Controls
{
    /// <summary>
    /// Interaction logic for TextBoxWithCompletionOptions.xaml
    /// </summary>
    public partial class TextBoxWithCompletionOptions : UserControl
    {
        #region Initializer
        public TextBoxWithCompletionOptions()
        {
            InitializeComponent();

            // bind the FilteredItemsSource.ItemsSource property
            BindingOperations.SetBinding(FilteredItemsSource, FilteredTextItemsSource.ItemsSourceProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(CompletionOptionsSource)),
                Mode = BindingMode.OneWay,
            });
            // bind the FilteredItemsSource.StringComparison property
            BindingOperations.SetBinding(FilteredItemsSource, FilteredTextItemsSource.StringComparisonProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(StringComparison)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
        }
        #endregion Initializer

        #region Fields
        private int _lastOptionsListViewSelectedIndex = 0;
        #endregion Fields

        #region Events
        #region SuggestionClickedEvent
        /// <summary>
        /// Event arguments for the <see cref="SuggestionClickedEventHandler"/> event type.
        /// </summary>
        public sealed class SuggestionClickedEventArgs : RoutedEventArgs
        {
            internal SuggestionClickedEventArgs(RoutedEvent routedEvent, object source, string suggestionText) : base(routedEvent, source)
            {
                SuggestionText = suggestionText;
            }

            /// <summary>
            /// Gets the suggestion text that was clicked.
            /// </summary>
            public string SuggestionText { get; }
        }
        public delegate void SuggestionClickedEventHandler(object sender, SuggestionClickedEventArgs e);
        /// <summary>
        /// <see cref="RoutedEvent"/> definition for <see cref="SuggestionClicked"/>.
        /// </summary>
        public static readonly RoutedEvent SuggestionClickedEvent = EventManager.RegisterRoutedEvent(
            nameof(SuggestionClicked),
            RoutingStrategy.Bubble,
            typeof(SuggestionClickedEventHandler),
            typeof(TextBoxWithCompletionOptions));
        /// <summary>
        /// Occurs when an item in the popup suggestion box is clicked.
        /// </summary>
        public event SuggestionClickedEventHandler SuggestionClicked
        {
            add => AddHandler(SuggestionClickedEvent, value);
            remove => RemoveHandler(SuggestionClickedEvent, value);
        }
        private void NotifySuggestionClicked(string suggestionText) => RaiseEvent(new SuggestionClickedEventArgs(SuggestionClickedEvent, OptionsListView, suggestionText));
        #endregion SuggestionClickedEvent

        #region CommittedTextEvent
        /// <summary>
        /// Event arguments for the <see cref="CommittedTextEventHandler"/> event type.
        /// </summary>
        public sealed class CommittedTextEventArgs : RoutedEventArgs
        {
            internal CommittedTextEventArgs(RoutedEvent routedEvent, object source, string text) : base(routedEvent, source)
            {
                Text = text;
            }

            /// <summary>
            /// Gets the text that is in the textbox when the enter key was pressed.
            /// </summary>
            public string Text { get; }
        }
        public delegate void CommittedTextEventHandler(object sender, CommittedTextEventArgs e);
        /// <summary>
        /// <see cref="RoutedEvent"/> definition for <see cref="CommittedText"/>.
        /// </summary>
        public static readonly RoutedEvent CommittedTextEvent = EventManager.RegisterRoutedEvent(
            nameof(CommittedText),
            RoutingStrategy.Bubble,
            typeof(CommittedTextEventHandler),
            typeof(TextBoxWithCompletionOptions));
        /// <summary>
        /// Occurs when the Enter key was pressed, indicating that the current text should be committed to whatever is consuming it.
        /// </summary>
        public event CommittedTextEventHandler CommittedText
        {
            add => AddHandler(CommittedTextEvent, value);
            remove => RemoveHandler(CommittedTextEvent, value);
        }
        private void NotifyCommittedText(string text) => RaiseEvent(new CommittedTextEventArgs(CommittedTextEvent, FilterTextBox, text));
        #endregion CommittedTextEvent

        #region BackPressedEvent
        /// <summary>
        /// <see cref="RoutedEvent"/> definition for <see cref="BackPressed"/>.
        /// </summary>
        public static readonly RoutedEvent BackPressedEvent = EventManager.RegisterRoutedEvent(
            nameof(BackPressed),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TextBoxWithCompletionOptions));
        /// <summary>
        /// Occurs when the backspace key is pressed when the textbox is already empty.
        /// </summary>
        public event RoutedEventHandler BackPressed
        {
            add => AddHandler(BackPressedEvent, value);
            remove => RemoveHandler(BackPressedEvent, value);
        }
        private void NotifyBackPressed() => RaiseEvent(new(BackPressedEvent, FilterTextBox));
        #endregion BackPressedEvent
        #endregion Events

        #region Properties
        protected FilteredTextItemsSource FilteredItemsSource => (FilteredTextItemsSource)FindResource(nameof(FilteredItemsSource));
        #endregion Properties

        #region CompletionOptionsSourceProperty
        public static readonly DependencyProperty CompletionOptionsSourceProperty = DependencyProperty.Register(
            nameof(CompletionOptionsSource),
            typeof(IEnumerable<string>),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(null));
        public IEnumerable<string> CompletionOptionsSource
        {
            get => (IEnumerable<string>)GetValue(CompletionOptionsSourceProperty);
            set => SetValue(CompletionOptionsSourceProperty, value);
        }
        #endregion CompletionOptionsSourceProperty

        #region TextProperty
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(string.Empty, OnTextPropertyChanged));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var inst = (TextBoxWithCompletionOptions)d;

            inst.FilteredItemsSource.FilterText = (string)e.NewValue;
        }
        #endregion TextProperty

        #region StringComparisonProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="StringComparison"/>.
        /// </summary>
        public static readonly DependencyProperty StringComparisonProperty = DependencyProperty.Register(
            nameof(StringComparison),
            typeof(StringComparison),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(StringComparison.OrdinalIgnoreCase));
        /// <summary>
        /// Gets or sets the string comparison type to use when matching the FilterText to items.
        /// </summary>
        public StringComparison StringComparison
        {
            get => (StringComparison)GetValue(StringComparisonProperty);
            set => SetValue(StringComparisonProperty, value);
        }
        #endregion StringComparisonProperty

        #region BackPressedRequiredModifiersProperty
        public static DependencyProperty BackPressedRequiredModifiersProperty = DependencyProperty.Register(
            nameof(BackPressedRequiredModifiers),
            typeof(ModifierKeys),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(ModifierKeys.Control));
        [TypeConverter(typeof(EnumConverter))]
        public ModifierKeys BackPressedRequiredModifiers
        {
            get => (ModifierKeys)GetValue(BackPressedRequiredModifiersProperty);
            set => SetValue(BackPressedRequiredModifiersProperty, value);
        }
        #endregion BackPressedRequiredModifiersProperty

        #region TextBoxSelectionBrushProperty
        public static readonly DependencyProperty TextBoxSelectionBrushProperty = DependencyProperty.Register(
            nameof(TextBoxSelectionBrush),
            typeof(Brush),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(TextBox.SelectionBrushProperty.DefaultMetadata.DefaultValue));
        public Brush TextBoxSelectionBrush
        {
            get => (Brush)GetValue(TextBoxSelectionBrushProperty);
            set => SetValue(TextBoxSelectionBrushProperty, value);
        }
        #endregion TextBoxSelectionBrushProperty

        #region TextBoxPaddingProperty
        public static readonly DependencyProperty TextBoxPaddingProperty = DependencyProperty.Register(
            nameof(TextBoxPadding),
            typeof(Thickness),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(TextBox.PaddingProperty.DefaultMetadata.DefaultValue));
        public Thickness TextBoxPadding
        {
            get => (Thickness)GetValue(TextBoxPaddingProperty);
            set => SetValue(TextBoxPaddingProperty, value);
        }
        #endregion TextBoxPaddingProperty

        #region TextBoxBorderThicknessProperty
        public static readonly DependencyProperty TextBoxBorderThicknessProperty = DependencyProperty.Register(
            nameof(TextBoxBorderThickness),
            typeof(Thickness),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(TextBox.BorderThicknessProperty.DefaultMetadata.DefaultValue));
        public Thickness TextBoxBorderThickness
        {
            get => (Thickness)GetValue(TextBoxBorderThicknessProperty);
            set => SetValue(TextBoxBorderThicknessProperty, value);
        }
        #endregion TextBoxBorderThicknessProperty

        #region TextBoxBorderBrushProperty
        public static readonly DependencyProperty TextBoxBorderBrushProperty = DependencyProperty.Register(
            nameof(TextBoxBorderBrush),
            typeof(Brush),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(TextBox.BorderBrushProperty.DefaultMetadata.DefaultValue));
        public Brush TextBoxBorderBrush
        {
            get => (Brush)GetValue(TextBoxBorderBrushProperty);
            set => SetValue(TextBoxBorderBrushProperty, value);
        }
        #endregion TextBoxBorderBrushProperty

        #region TextBoxForegroundProperty
        public static readonly DependencyProperty TextBoxForegroundProperty = DependencyProperty.Register(
            nameof(TextBoxForeground),
            typeof(Brush),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(TextBox.ForegroundProperty.DefaultMetadata.DefaultValue));
        public Brush TextBoxForeground
        {
            get => (Brush)GetValue(TextBoxForegroundProperty);
            set => SetValue(TextBoxForegroundProperty, value);
        }
        #endregion TextBoxForegroundProperty

        #region TextBoxBackgroundProperty
        public static readonly DependencyProperty TextBoxBackgroundProperty = DependencyProperty.Register(
            nameof(TextBoxBackground),
            typeof(Brush),
            typeof(TextBoxWithCompletionOptions),
            new PropertyMetadata(TextBox.BackgroundProperty.DefaultMetadata.DefaultValue));
        public Brush TextBoxBackground
        {
            get => (Brush)GetValue(TextBoxBackgroundProperty);
            set => SetValue(TextBoxBackgroundProperty, value);
        }
        #endregion TextBoxBackgroundProperty

        #region EventHandlers

        #region OptionsPopup
        private void OptionsPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var popup = (System.Windows.Controls.Primitives.Popup)sender;
            var window = Window.GetWindow(popup);

            // fix position when the window's location changes:
            window.LocationChanged += (s, e) =>
            {
                popup.HorizontalOffset += 1;
                popup.HorizontalOffset -= 1;
            };
            // fix position when the window's size changes:
            window.SizeChanged += (s, e) =>
            {
                popup.HorizontalOffset += 1;
                popup.HorizontalOffset -= 1;
            };
        }
        #endregion OptionsPopup

        #region FilterTextBox
        private void FilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
            case Key.Back:
                if (FilterTextBox.Text.Length > 0) break;

                if (!Keyboard.Modifiers.Equals(BackPressedRequiredModifiers)) break;

                NotifyBackPressed();
                e.Handled = true;
                break;
            case Key.Enter:
                NotifyCommittedText(FilterTextBox.Text);
                break;
            }
        }
        private void FilterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
            case Key.Up:
                { // decrement selected suggestion index:
                    var index = OptionsListView.SelectedIndex;
                    // calculate the previous index:
                    if (index > 0)
                        index--;
                    else
                        index = OptionsListView.Items.Count - 1;

                    // set the selected index:
                    OptionsListView.SelectionChanged -= OptionsListView_SelectionChanged;
                    OptionsListView.SelectedIndex = index;
                    OptionsListView.SelectionChanged += OptionsListView_SelectionChanged;

                    e.Handled = true; //< key press has been handled
                    break;
                }
            case Key.Down:
                { // increment selected suggestion index:
                    var index = OptionsListView.SelectedIndex;
                    // calculate the next index:
                    if (index < OptionsListView.Items.Count - 1)
                        index++;
                    else
                        index = 0;

                    // set the selected index:
                    OptionsListView.SelectionChanged -= OptionsListView_SelectionChanged;
                    OptionsListView.SelectedIndex = index;
                    OptionsListView.SelectionChanged += OptionsListView_SelectionChanged;

                    e.Handled = true; //< key press has been handled
                    break;
                }
            case Key.Left:
                { // save & clear selected suggestion index:
                    if (OptionsListView.SelectedIndex == -1) break; //< if nothing is selected, ignore key press

                    _lastOptionsListViewSelectedIndex = OptionsListView.SelectedIndex;

                    // set the selected index:
                    OptionsListView.SelectionChanged -= OptionsListView_SelectionChanged;
                    OptionsListView.SelectedIndex = -1;
                    OptionsListView.SelectionChanged += OptionsListView_SelectionChanged;

                    e.Handled = true; //< key press has been handled
                    break;
                }
            case Key.Right:
                { // recall selected suggestion index:
                    if (OptionsListView.SelectedIndex != -1) break; //< if something is selected, ignore key press

                    // make sure the saved index is valid:
                    if (_lastOptionsListViewSelectedIndex >= OptionsListView.Items.Count)
                        _lastOptionsListViewSelectedIndex = 0;

                    // set the selected index:
                    OptionsListView.SelectionChanged -= OptionsListView_SelectionChanged;
                    OptionsListView.SelectedIndex = _lastOptionsListViewSelectedIndex;
                    OptionsListView.SelectionChanged += OptionsListView_SelectionChanged;

                    e.Handled = true; //< key press has been handled
                    break;
                }
            case Key.Tab:
                { // insert a suggestion into the FilterTextBox
                    if (OptionsListView.SelectedIndex != -1)
                    { // a suggestion is selected:
                        FilterTextBox.Text = (string)OptionsListView.SelectedItem;
                        FilterTextBox.SelectionStart = FilterTextBox.Text.Length;
                        e.Handled = true;
                    }
                    else if (OptionsListView.Items.Count == 1)
                    { // only 1 suggestion is in the list:
                        FilterTextBox.Text = (string)OptionsListView.Items[0];
                        FilterTextBox.SelectionStart = FilterTextBox.Text.Length;
                        e.Handled = true; //< key press has been handled
                    }
                    break;
                }
            case Key.Enter:
                { // insert the selected suggestion into the FilterTextBox
                    if (OptionsListView.SelectedIndex == -1) break;

                    // insert the selected suggestion:
                    FilterTextBox.Text = (string)OptionsListView.SelectedItem;
                    FilterTextBox.SelectionStart = FilterTextBox.Text.Length;
                    //< Do not set handled here, we want the KeyUp event to fire!
                    break;
                }
            case Key.Escape:
                { // unfocus the textbox & close the popup:
                    if (Window.GetWindow(this) is Window window)
                    { // set focus to window
                        // set logical focus
                        FocusManager.SetFocusedElement(FocusManager.GetFocusScope(FilterTextBox), window);
                        // set keyboard focus
                        Keyboard.Focus(window);
                    }
                    else
                    { // completely clear focus
                        // clear logical focus
                        FocusManager.SetFocusedElement(FocusManager.GetFocusScope(FilterTextBox), null);
                        // clear keyboard focus
                        Keyboard.ClearFocus();
                    }
                    e.Handled = true;
                    break;
                }
            }
        }
        #endregion FilterTextBox

        #region OptionsListView
        private void OptionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTextBox.Focus(); //< prevent focus from changing

            if (e.AddedItems.Count == 0) return;

            NotifySuggestionClicked((string)e.AddedItems[0]!);
        }
        #endregion OptionsListView

        #region ListViewItem
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            // prevent mouse from accidentally selecting other items when the popup position changes:
            OptionsListView.ReleaseMouseCapture();
        }
        #endregion ListViewItem

        #endregion EventHandlers
    }
}
