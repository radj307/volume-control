using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Toastify.Model;

namespace Toastify.View
{
    public partial class DebugView : Window
    {
        public DebugView()
        {
            this.InitializeComponent();
        }

        private void ButtonPrintCurrentHotkeys_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("\nCURRENT HOTKEYS:");
            foreach (var h in Hotkey.Hotkeys)
                Debug.WriteLine(h.ToString());
            Debug.WriteLine("\n");
        }

        private void DebugView_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
        }
    }
}