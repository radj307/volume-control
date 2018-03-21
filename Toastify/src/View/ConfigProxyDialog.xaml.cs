using System.Windows;
using Toastify.ViewModel;

namespace Toastify.View
{
    public partial class ConfigProxyDialog : Window
    {
        public ConfigProxyDialog()
        {
            this.InitializeComponent();
            this.DataContext = new ConfigProxyDialogViewModel();
        }
    }
}