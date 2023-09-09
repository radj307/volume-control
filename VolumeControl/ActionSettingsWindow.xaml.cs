using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VolumeControl.Core.Input.Actions;
using VolumeControl.ViewModels;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for ActionSettingsWindow.xaml
    /// </summary>
    public partial class ActionSettingsWindow : Window
    {
        public ActionSettingsWindow()
        {
            InitializeComponent();
        }

        public ActionSettingsWindowVM VM => (this.DataContext as ActionSettingsWindowVM)!;

        private void ListBoxRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            var listBox = (ListBox)button.Tag;
            var setting = (HotkeyActionSetting)listBox.DataContext;

            if (setting?.Value is not ActionTargetSpecifier list) return;

            list.Targets.Remove(button.DataContext);
        }

        private void ApplicationCommands_Close_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        /// <summary>
        /// Creates a new target override entry with the text in the textbox when the Enter key is pressed.
        /// </summary>
        private void AddTargetTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                if (textBox.Text.Trim().Length == 0) return; //< if the text is blank, don't add it to the list

                var listBox = (ListBox)textBox.Tag;
                var setting = (HotkeyActionSetting)listBox.DataContext;

                if (setting?.Value is not ActionTargetSpecifier list) return;

                list.Targets.Add(new() { Value = textBox.Text });

                textBox.Text = string.Empty;
            }
        }
        /// <summary>
        /// Creates a new target override entry with the text in the textbox when the control loses focus.
        /// </summary>
        private void AddTargetTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text.Trim().Length == 0) return; //< if the text is blank, don't add it to the list

            var listBox = (ListBox)textBox.Tag;
            var setting = (HotkeyActionSetting)listBox.DataContext;

            if (setting?.Value is not ActionTargetSpecifier list) return;

            list.Targets.Add(new() { Value = textBox.Text });

            textBox.Text = string.Empty;
        }
    }
}
