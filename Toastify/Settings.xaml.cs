using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Toastify
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        List<Key> modifierKeys = new List<Key> { Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.Right, Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin, Key.System };
        private void TextHotKey_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var tb = sender as TextBox;
            tb.Text = string.Empty;
            tb.Tag = null;

            if (modifierKeys.Contains(e.Key))
                return;
            if (Keyboard.Modifiers == ModifierKeys.None)
                return;

            var hotkey = new Hotkey();
            hotkey.Win = ((Keyboard.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows);
            hotkey.Ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
            hotkey.Alt = ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt);
            hotkey.Shift = ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
            hotkey.Key = e.Key;

            tb.Text = hotkey.ToString();
            tb.Tag = hotkey;
        }
    }
}
