using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VolumeControl.WPF.Extensions;

namespace VolumeControl.WPF.CustomMessageBox
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        #region Constructor
        internal CustomMessageBox(CustomMessageBoxData data)
        {
            Data = data;

            Data.Appearance.PropertyChanged += this.Appearance_PropertyChanged;

            InitializeComponent();
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the <see cref="CustomMessageBoxData"/> object associated with this <see cref="CustomMessageBox"/> window.
        /// </summary>
        public CustomMessageBoxData Data { get; }
        #endregion Properties

        #region Methods

        #region Show
        /// <summary>
        /// Creates &amp; displays a new <see cref="CustomMessageBox"/> dialog window.
        /// </summary>
        /// <param name="data">The <see cref="CustomMessageBoxData"/> instance that defines what is shown in the window.</param>
        /// <returns>The clicked <see cref="CustomMessageBoxButton"/> instance when the user clicked a button; otherwise, the DefaultResult of the specified <paramref name="data"/> object.<br/>You can convert it to a <see cref="MessageBoxResult"/> with <see cref="CustomMessageBoxButton.ToMessageBoxResult(bool)"/>.</returns>
        public static CustomMessageBoxButton? Show(CustomMessageBoxData data) => data.Show();
        #endregion Show

        #region (Private) GetMaxWidth
        private double GetMaxWidth()
        {
            if (Data.Appearance.MaxWidth.HasValue)
                return Data.Appearance.MaxWidth.Value;

            var screenWidth = this.GetCurrentScreen().Bounds.Width;
            if (Data.Appearance.MaxWidthScreenPercentage.HasValue)
                return screenWidth * Data.Appearance.MaxWidthScreenPercentage.Value;
            return screenWidth;

        }
        #endregion (Private) GetMaxWidth

        #endregion Methods

        #region EventHandlers

        #region Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        { // drag move the window
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                e.Handled = true;
            }
        }
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        { // drag move the window if ALT is held down
            if (e.LeftButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {
                this.DragMove();
                e.Handled = true;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // set the max width
            MaxWidth = GetMaxWidth();

            // focus the default result button
            if (Data.DefaultResult is CustomMessageBoxButton cmbButton
                && ButtonsPanel.ItemContainerGenerator.ContainerFromItem(cmbButton) is DependencyObject itemContainer
                && VisualTreeHelper.GetChildrenCount(itemContainer) > 0)
            {
                var resultButton = (Button)VisualTreeHelper.GetChild(itemContainer, 0);
                resultButton.Focus();
            }
        }
        #endregion Window

        #region CloseWindowButton
        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion CloseWindowButton

        #region DynamicButton
        private void ResultButton_Click(object sender, RoutedEventArgs e)
        { // set the result & close
            var button = (Button)sender;
            var cmbButton = (CustomMessageBoxButton)button.DataContext;
            Data.SetResult(cmbButton);
            Close();
        }
        #endregion DynamicButton

        #region Data
        private void Appearance_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(CustomMessageBoxStyle.MaxWidth), System.StringComparison.Ordinal)
                || e.PropertyName.Equals(nameof(CustomMessageBoxStyle.MaxWidthScreenPercentage), System.StringComparison.Ordinal))
            { // update the max width
                MaxWidth = GetMaxWidth();
            }
        }
        #endregion Data

        #endregion EventHandlers
    }
}
