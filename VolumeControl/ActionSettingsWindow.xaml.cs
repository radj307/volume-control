using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VolumeControl.Core;
using VolumeControl.Core.Generics;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Actions;
using VolumeControl.SDK;
using VolumeControl.ViewModels;
using VolumeControl.WPF.Collections;

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

                if (setting?.Value is not ObservableImmutableList<TargetInfoVM> list) return;

                string currentSelection = VCAPI.Default.Settings.Target.GetProcessName();
                if (!list.Any(i => i.ProcessName.Equals(currentSelection)))
                    list.Add(new(currentSelection));
                else
                    list.Add(new()); //< add a blank entry
            }
        }

        private void ListBoxRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                var listBox = (b.CommandParameter as ListBox)!;
                var setting = listBox.DataContext as HotkeyActionSetting;

                if (setting?.Value is not ObservableImmutableList<TargetInfoVM> list) return;

                list.Remove(b.Tag);
            }
        }

        private void ApplicationCommands_Close_Executed(object sender, ExecutedRoutedEventArgs e) => Close();
    }
}
