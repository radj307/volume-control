using System.Drawing;
using System.Media;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Toastify.View
{
    public partial class CustomMessageBoxWindow : Window
    {
        private SystemSound sound;

        internal string Caption
        {
            get { return this.Title; }
            set { this.Title = value; }
        }

        internal string Message
        {
            get { return this.TextBlockMessage.Text; }
            set { this.TextBlockMessage.Text = value; }
        }

        internal string OkButtonText
        {
            get { return this.LabelOk.Content.ToString(); }
            set { this.LabelOk.Content = TryAddKeyboardAccellerator(value); }
        }

        internal string CancelButtonText
        {
            get { return this.LabelCancel.Content.ToString(); }
            set { this.LabelCancel.Content = TryAddKeyboardAccellerator(value); }
        }

        internal string YesButtonText
        {
            get { return this.LabelYes.Content.ToString(); }
            set { this.LabelYes.Content = TryAddKeyboardAccellerator(value); }
        }

        internal string NoButtonText
        {
            get { return this.LabelNo.Content.ToString(); }
            set { this.LabelNo.Content = TryAddKeyboardAccellerator(value); }
        }

        public MessageBoxResult Result { get; set; }

        internal CustomMessageBoxWindow(string message) : this(message, null)
        {
        }

        internal CustomMessageBoxWindow(string message, string caption) : this(message, caption, MessageBoxButton.OK)
        {
        }

        internal CustomMessageBoxWindow(string message, string caption, MessageBoxButton button) : this(message, caption, button, MessageBoxImage.None)
        {
        }

        internal CustomMessageBoxWindow(string message, string caption, MessageBoxImage image) : this(message, caption, MessageBoxButton.OK, image)
        {
        }

        internal CustomMessageBoxWindow(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            this.InitializeComponent();

            this.Message = message;
            this.Caption = caption;
            this.ImageMessageBox.Visibility = Visibility.Collapsed;

            this.DisplayButtons(button);
            this.DisplayImage(image);
            this.SetSound(image);
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    // Hide all but OK, Cancel
                    this.ButtonOk.Visibility = Visibility.Visible;
                    this.ButtonOk.Focus();
                    this.ButtonCancel.Visibility = Visibility.Visible;

                    this.ButtonYes.Visibility = Visibility.Collapsed;
                    this.ButtonNo.Visibility = Visibility.Collapsed;
                    break;

                case MessageBoxButton.YesNo:
                    // Hide all but Yes, No
                    this.ButtonYes.Visibility = Visibility.Visible;
                    this.ButtonYes.Focus();
                    this.ButtonNo.Visibility = Visibility.Visible;

                    this.ButtonOk.Visibility = Visibility.Collapsed;
                    this.ButtonCancel.Visibility = Visibility.Collapsed;
                    break;

                case MessageBoxButton.YesNoCancel:
                    // Hide only OK
                    this.ButtonYes.Visibility = Visibility.Visible;
                    this.ButtonYes.Focus();
                    this.ButtonNo.Visibility = Visibility.Visible;
                    this.ButtonCancel.Visibility = Visibility.Visible;

                    this.ButtonOk.Visibility = Visibility.Collapsed;
                    break;

                default:
                    // Hide all but OK
                    this.ButtonOk.Visibility = Visibility.Visible;
                    this.ButtonOk.Focus();

                    this.ButtonYes.Visibility = Visibility.Collapsed;
                    this.ButtonNo.Visibility = Visibility.Collapsed;
                    this.ButtonCancel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void DisplayImage(MessageBoxImage image)
        {
            Icon icon;

            switch (image)
            {
                case MessageBoxImage.None:
                    return;

                case MessageBoxImage.Exclamation:       // Enumeration value 48 - also covers "Warning"
                    icon = SystemIcons.Exclamation;
                    break;

                case MessageBoxImage.Error:             // Enumeration value 16, also covers "Hand" and "Stop"
                    icon = SystemIcons.Hand;
                    break;

                case MessageBoxImage.Information:       // Enumeration value 64 - also covers "Asterisk"
                    icon = SystemIcons.Information;
                    break;

                case MessageBoxImage.Question:
                    icon = SystemIcons.Question;
                    break;

                default:
                    icon = SystemIcons.Information;
                    break;
            }

            this.ImageMessageBox.Source = ToImageSource(icon);
            this.ImageMessageBox.Visibility = Visibility.Visible;
        }

        private void SetSound(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Exclamation: // Enumeration value 48 - also covers "Warning"
                    this.sound = SystemSounds.Exclamation;
                    break;

                case MessageBoxImage.Error: // Enumeration value 16, also covers "Hand" and "Stop"
                    this.sound = SystemSounds.Hand;
                    break;

                case MessageBoxImage.Information: // Enumeration value 64 - also covers "Asterisk"
                    this.sound = SystemSounds.Asterisk;
                    break;

                case MessageBoxImage.Question:
                    this.sound = SystemSounds.Question;
                    break;

                default:
                    break;
            }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.OK;
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.Cancel;
            this.Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            this.Result = MessageBoxResult.No;
            this.Close();
        }

        private void CustomMessageBoxWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.sound?.Play();
        }

        #region Util

        /// <summary>
        /// Keyboard Accellerators are used in Windows to allow easy shortcuts to controls like Buttons and
        /// MenuItems. These allow users to press the Alt key, and a shortcut key will be highlighted on the
        /// control. If the user presses that key, that control will be activated.
        /// This method checks a string if it contains a keyboard accellerator. If it doesn't, it adds one to the
        /// beginning of the string. If there are two strings with the same accellerator, Windows handles it.
        /// The keyboard accellerator character for WPF is underscore (_). It will not be visible.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string TryAddKeyboardAccellerator(string input)
        {
            const string accellerator = "_"; // This is the default WPF accellerator symbol - used to be & in WinForms

            // If it already contains an accellerator, do nothing
            if (input.Contains(accellerator))
                return input;

            return accellerator + input;
        }

        private static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        #endregion Util
    }
}