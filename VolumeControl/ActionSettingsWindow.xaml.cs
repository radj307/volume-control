using System;
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
    }
}
