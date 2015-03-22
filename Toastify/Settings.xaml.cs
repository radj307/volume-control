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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Forms;
using System.Globalization;

namespace Toastify
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public SettingsXml settings;
        private Toast toast;

        private List<System.Windows.Input.Key> modifierKeys = new List<System.Windows.Input.Key> { System.Windows.Input.Key.LeftCtrl, System.Windows.Input.Key.RightCtrl, System.Windows.Input.Key.LeftAlt, System.Windows.Input.Key.Right, System.Windows.Input.Key.LeftShift, System.Windows.Input.Key.RightShift, System.Windows.Input.Key.LWin, System.Windows.Input.Key.RWin, System.Windows.Input.Key.System };

        private static Settings _current;

        public static void Launch(Toast toast)
        {
            if (_current != null)
            {
                _current.Activate();
            }
            else 
            {
                new Settings(toast).ShowDialog();
            }
        }

        public Settings(Toast toast)
        {
            this.settings = SettingsXml.Current.Clone();
            this.toast = toast;

            InitializeComponent();

            //Data context initialisation
            GeneralGrid.DataContext = this.settings;

            //Slider initialisation
            try
            {
                slTopColor.Value = byte.Parse(settings.ToastColorTop.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                slBottomColor.Value = byte.Parse(settings.ToastColorBottom.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                slBorderColor.Value = byte.Parse(settings.ToastBorderColor.Substring(1, 2), NumberStyles.AllowHexSpecifier);
            }
            catch { }

            if (_current == null)
                _current = this;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_current == this)
                _current = null;
        }

        //Change Color button click events
        private void bChangeColorTop_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            string alpha = settings.ToastColorTop.Substring(1, 2);
            MyDialog.Color = HexToColor(settings.ToastColorTop);
            MyDialog.ShowDialog();
            settings.ToastColorTop = "#" + alpha + MyDialog.Color.R.ToString("X2") + MyDialog.Color.G.ToString("X2") + MyDialog.Color.B.ToString("X2");
        }

        private void bChangeColorBottom_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            string alpha = settings.ToastColorBottom.Substring(1, 2);
            MyDialog.Color = HexToColor(settings.ToastColorBottom);
            MyDialog.ShowDialog();
            settings.ToastColorBottom = "#" + alpha + MyDialog.Color.R.ToString("X2") + MyDialog.Color.G.ToString("X2") + MyDialog.Color.B.ToString("X2");
        }

        private void bChangeBorderColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            string alpha = settings.ToastBorderColor.Substring(1, 2);
            MyDialog.Color = HexToColor(settings.ToastBorderColor);
            MyDialog.ShowDialog();
            settings.ToastBorderColor = "#" + alpha + MyDialog.Color.R.ToString("X2") + MyDialog.Color.G.ToString("X2") + MyDialog.Color.B.ToString("X2");
        }

        //Default and Save blick events
        private void bDefault_Click(object sender, RoutedEventArgs e)
        {
            settings.Default();
            SaveAndApplySettings();
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            SaveAndApplySettings();
        }

        private void SaveAndApplySettings()
        {
            settings.Save(replaceCurrent: true);

            toast.InitToast();
            toast.DisplayAction(SpotifyAction.SettingsSaved, "");
        }

        //Text box Mouse Wheel events 
        private void tbCornerTopLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                settings.ToastBorderCornerRadiusTopLeft += 0.1;
            }
            else if (settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                settings.ToastBorderCornerRadiusTopLeft -= 0.1;
        }

        private void tbCornerTopRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.ToastBorderCornerRadiusTopRight += 0.1;
            else if (settings.ToastBorderCornerRadiusTopLeft >= 0.1)
                settings.ToastBorderCornerRadiusTopRight -= 0.1;
        }

        private void tbCornerBottomRight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.ToastBorderCornerRadiusBottomRight += 0.1;
            else if (settings.ToastBorderCornerRadiusBottomRight >= 0.1)
                settings.ToastBorderCornerRadiusBottomRight -= 0.1;
        }

        private void tbCornerBottomLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.ToastBorderCornerRadiusBottomLeft += 0.1;
            else if (settings.ToastBorderCornerRadiusBottomLeft >= 0.1)
                settings.ToastBorderCornerRadiusBottomLeft -= 0.1;
        }

        private void tbFadeOutTime_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.FadeOutTime += 10;
            else if (settings.FadeOutTime >= 10)
                settings.FadeOutTime -= 10;
        }

        private void tbBorderThickness_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.ToastBorderThickness++;
            else if (settings.ToastBorderThickness >= 1)
                settings.ToastBorderThickness--;
        }

        private void tbToastWidth_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.ToastWidth += 5;
            else if (settings.ToastWidth >= 205)
                settings.ToastWidth -= 5;
        }

        private void tbToastHeight_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.ToastHeight += 5;
            else if (settings.ToastHeight >= 70)
                settings.ToastHeight -= 5;
        }

        private void tbPositionLeft_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.PositionLeft++;
            else if (settings.PositionLeft > 0)
                settings.PositionLeft--;
        }

        private void tbPositionTop_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                settings.PositionTop++;
            else if (settings.PositionTop > 0)
                settings.PositionTop--;
        }

        //Slider value changed events
        private void slTopColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string transparency = Convert.ToByte(slTopColor.Value).ToString("X2");
            settings.ToastColorTop = "#" + transparency + settings.ToastColorTop.Substring(3);
        }

        private void slBottomColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string transparency = Convert.ToByte(slBottomColor.Value).ToString("X2");
            settings.ToastColorBottom = "#" + transparency + settings.ToastColorBottom.Substring(3);
        }

        private void slBorderColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string transparency = Convert.ToByte(slBorderColor.Value).ToString("X2");
            settings.ToastBorderColor = "#" + transparency + settings.ToastBorderColor.Substring(3);
        }

        // Hexadecimal to Color converter
        public static System.Drawing.Color HexToColor(string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            byte alpha = 0;
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            if (hexColor.Length == 8)
            {
                //#RRGGBB
                alpha = byte.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                red = byte.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                green = byte.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                blue = byte.Parse(hexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
            }

            return System.Drawing.Color.FromArgb(alpha, red, green, blue);
        }

        private void txtSingleKey_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.Key;

            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            txtSingleKey.Text = key.ToString();

            Hotkey hotkey = lstHotKeys.SelectedItem as Hotkey;

            if (hotkey != null)
            {
                hotkey.Key = key;
            }
        }

        private void lstHotKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Hotkey hotkey = lstHotKeys.SelectedItem as Hotkey;

            if (hotkey != null)
            {
                txtSingleKey.Text = hotkey.Key.ToString();
            }
        }

        private void btnSaveTrackToFilePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (SettingsXml.Current.SaveTrackToFilePath != null)
            {
                dialog.FileName = SettingsXml.Current.SaveTrackToFilePath;
            }

            dialog.CheckPathExists = true;
            dialog.CheckFileExists = false;
            dialog.ShowReadOnly = false;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                settings.SaveTrackToFilePath = dialog.FileName;
            }
        }

    }
}
