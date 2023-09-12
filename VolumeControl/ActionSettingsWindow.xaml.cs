using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Interfaces;
using VolumeControl.SDK;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for ActionSettingsWindow.xaml
    /// </summary>
    public partial class ActionSettingsWindow : Window, INotifyPropertyChanged
    {
        #region Initializers
        public ActionSettingsWindow()
        {
            InitializeComponent();
        }
        public ActionSettingsWindow(Window owner, IBindableHotkey hotkey) : this()
        {
            Owner = owner;
            Title = hotkey.Name;
            Hotkey = hotkey;
        }
        #endregion Initializers

        #region Events
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => this.RaisePropertyChanged(propertyName);
        #endregion Events

        #region Fields
        private int _lastSelectedSuggestionIndex = 0;
        #endregion Fields

        #region Properties
        public IBindableHotkey? Hotkey
        {
            get => _hotkey;
            set
            {
                _hotkey = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ActionSettings));
            }
        }
        private IBindableHotkey? _hotkey;
        public ObservableImmutableList<IHotkeyActionSetting>? ActionSettings => this.Hotkey?.ActionSettings;
        #endregion Properties

        #region EventHandlers

        #region Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.ChangedButton.Equals(MouseButton.Left)) return;

            this.DragMove();
            e.Handled = true;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            VCAPI.Default.Settings.Save();
        }
        #endregion Window

        #region ListBoxRemoveButton
        private void ListBoxRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            var listBox = (ListBox)button.Tag;
            var setting = (HotkeyActionSetting)listBox.DataContext;

            if (setting?.Value is not ActionTargetSpecifier list) return;

            list.Targets.Remove(button.DataContext);
        }
        #endregion ListBoxRemoveButton

        #region ApplicationCommands
        /// <summary>
        /// Closes the window.
        /// </summary>
        private void ApplicationCommands_Close_Executed(object sender, ExecutedRoutedEventArgs e) => Close();
        #endregion ApplicationCommands

        #region CloseWindowButton
        /// <summary>
        /// Closes the window.
        /// </summary>
        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) => Close();
        #endregion CloseWindowButton

        #region AddTargetBox
        /// <summary>
        /// Creates a new target override entry with the text in the textbox when the Enter key is pressed.
        /// </summary>
        private void AddTargetTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
            case Key.Back:
                { // remove last item from the list:
                    // don't do anything if the CTRL modifier key is pressed (backspace word)
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) return;

                    var textBox = (TextBox)sender;
                    if (textBox.Text.Length != 0) return;

                    var listBox = (ListBox)textBox.DataContext;
                    if (listBox.Items.Count == 0) return;
                    var setting = (HotkeyActionSetting)listBox.DataContext;

                    if (setting?.Value is not ActionTargetSpecifier list) return;

                    list.Targets.Remove(listBox.Items[^1]);//< remove the last item in the list
                    break;
                }
            case Key.Enter:
                { // commit current text
                    var textBox = (TextBox)sender;

                    if (textBox.Text.Trim().Length == 0) return; //< if the text is blank, don't add it to the list

                    var listBox = (ListBox)textBox.DataContext;
                    var setting = (HotkeyActionSetting)listBox.DataContext;

                    if (setting?.Value is not ActionTargetSpecifier list) return;

                    if (VCAPI.Default.AudioSessionManager.FindSessionWithProcessName(textBox.Text, StringComparison.OrdinalIgnoreCase) is CoreAudio.AudioSession session)
                    {
                        list.Targets.Add(new() { Value = session.ProcessName });
                    }
                    else
                    {
                        list.Targets.Add(new() { Value = textBox.Text });
                    }

                    textBox.Text = string.Empty;
                    break;
                }
            default: break;
            }
        }
        private void AddTargetTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        { // we have to use PreviewKeyDown because the KeyDown event doesn't fire for the arrow keys
            switch (e.Key)
            {
            case Key.Up:
                { // decrement selected session:
                    var textBox = (TextBox)sender;
                    var suggestionsView = (ListView)textBox.Tag;

                    var index = suggestionsView.SelectedIndex;
                    if (index > 0)
                        index--;
                    else
                        index = suggestionsView.Items.Count - 1;

                    suggestionsView.SelectionChanged -= TargetSuggestionsView_SelectionChanged;

                    suggestionsView.SelectedIndex = index;

                    suggestionsView.SelectionChanged += TargetSuggestionsView_SelectionChanged;

                    e.Handled = true; //< this key press is handled, don't do anything else with it
                    break;
                }
            case Key.Down:
                { // increment selected suggestion:
                    var textBox = (TextBox)sender;
                    var suggestionsView = (ListView)textBox.Tag;

                    var index = suggestionsView.SelectedIndex;
                    if (index < suggestionsView.Items.Count - 1)
                        index++;
                    else
                        index = 0;

                    suggestionsView.SelectionChanged -= TargetSuggestionsView_SelectionChanged;

                    suggestionsView.SelectedIndex = index;

                    suggestionsView.SelectionChanged += TargetSuggestionsView_SelectionChanged;

                    e.Handled = true; //< this key press is handled, don't do anything else with it
                    break;
                }
            case Key.Left:
                { // save selected suggestion to memory & deselect:
                    var textBox = (TextBox)sender;
                    var suggestionsView = (ListView)textBox.Tag;
                    if (suggestionsView.SelectedIndex == -1) return; //< if nothing is selected, return

                    _lastSelectedSuggestionIndex = suggestionsView.SelectedIndex; //< update last index

                    suggestionsView.SelectionChanged -= TargetSuggestionsView_SelectionChanged;

                    suggestionsView.SelectedIndex = -1;

                    suggestionsView.SelectionChanged += TargetSuggestionsView_SelectionChanged;

                    e.Handled = true; //< this key press is handled, don't do anything else with it
                    break;
                }
            case Key.Right:
                { // recall selected suggestion:
                    var textBox = (TextBox)sender;
                    var suggestionsView = (ListView)textBox.Tag;
                    if (suggestionsView.SelectedIndex != -1) return; //< if something is selected, return

                    if (_lastSelectedSuggestionIndex >= suggestionsView.Items.Count)
                        _lastSelectedSuggestionIndex = 0; //< don't set SelectedIndex to a non-existent index

                    suggestionsView.SelectionChanged -= TargetSuggestionsView_SelectionChanged;

                    suggestionsView.SelectedIndex = _lastSelectedSuggestionIndex;

                    suggestionsView.SelectionChanged += TargetSuggestionsView_SelectionChanged;

                    e.Handled = true; //< this key press is handled, don't do anything else with it
                    break;
                }
            case Key.Tab:
                { // accept a suggestion:
                    var textBox = (TextBox)sender;
                    var suggestionsView = (ListView)textBox.Tag;

                    if (suggestionsView.SelectedIndex != -1)
                    { // an item is selected
                        textBox.Text = (string)suggestionsView.SelectedItem;
                        textBox.SelectionStart = textBox.Text.Length;
                        e.Handled = true; //< this key press is handled, don't do anything else with it
                    }
                    else if (suggestionsView.Items.Count == 1)
                    { // only 1 option is available
                        textBox.Text = (string)suggestionsView.Items[0];
                        textBox.SelectionStart = textBox.Text.Length;
                        e.Handled = true; //< this key press is handled, don't do anything else with it
                    }
                    break;
                }
            case Key.Enter: //< this is the only key that doesn't HAVE to be here; but doing this improves responsiveness
                { // insert selected suggestion:
                    var textBox = (TextBox)sender;
                    var suggestionsView = (ListView)textBox.Tag; //< use the currently selected suggestion instead, if available

                    if (suggestionsView.SelectedIndex != -1)
                    {
                        textBox.Text = (string)suggestionsView.SelectedItem;
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                    break;
                }
            default: break;
            }
        }
        #endregion AddTargetBox

        #region TargetSuggestionsView
        /// <summary>
        /// Inserts the selected suggestion into the text box.
        /// </summary>
        private void TargetSuggestionsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var listView = (ListView)sender;
            var textBox = (TextBox)listView.Tag;

            textBox.Text = (string)e.AddedItems[0]!;
            textBox.Focus();
            textBox.SelectionStart = textBox.Text?.Length ?? 0;
        }
        #endregion TargetSuggestionsView

        #region TargetSuggestionsPopup
        /// <summary>
        /// Fixes the positioning of the popup when the window moves or changes size.
        /// </summary>
        private void TargetSuggestionsPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var popup = (System.Windows.Controls.Primitives.Popup)sender;

            // fix position when the window's location changes:
            LocationChanged += (s, e) =>
            {
                popup.HorizontalOffset += 1;
                popup.HorizontalOffset -= 1;
            };
            // fix position when the window's size changes:
            SizeChanged += (s, e) =>
            {
                popup.HorizontalOffset += 1;
                popup.HorizontalOffset -= 1;
            };
        }
        #endregion TargetSuggestionsPopup

        #endregion EventHandlers
    }
}
