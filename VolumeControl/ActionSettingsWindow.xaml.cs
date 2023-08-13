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

        private void ListBoxAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                var listBox = (b.CommandParameter as ListBox)!;
                var setting = listBox.DataContext as HotkeyActionSetting;

                if (setting?.Value is not ActionTargetSpecifier list) return;

                list.AddNewTarget();
            }
        }

        private void ListBoxRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                var listBox = (b.CommandParameter as ListBox)!;
                var setting = listBox.DataContext as HotkeyActionSetting;

                if (setting?.Value is not ActionTargetSpecifier list) return;

                list.Targets.Remove(b.Tag);
            }
        }

        private void ApplicationCommands_Close_Executed(object sender, ExecutedRoutedEventArgs e) => Close();
    }
}
