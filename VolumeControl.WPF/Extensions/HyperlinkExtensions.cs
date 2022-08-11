using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using VolumeControl.Log;

namespace VolumeControl.WPF.Extensions
{
    /// <summary>
    /// Extends the <see cref="Hyperlink"/> class with additional <see cref="DependencyProperty"/> properties.<br/>
    /// To use this, first include the namespace:  
    /// <code>
    /// xmlns:extensions="clr-namespace:VolumeControl.WPF.Extensions;assembly=VolumeControl.WPF"
    /// </code>
    /// Then attach the property to a <see cref="Hyperlink"/> in XAML:
    /// <code>
    /// extensions:HyperlinkExtensions.OpensInBrowser="True"
    /// </code>
    /// </summary>
    public static class HyperlinkExtensions
    {
        #region Fields
        private const string PropertyName_OpensInBrowser = "OpensInBrowser";
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        #endregion Properties

        #region DependencyProperty
        /// <summary>
        /// Gets or sets whether this <see cref="Hyperlink"/> instance is 
        /// </summary>
        public static readonly DependencyProperty OpensInBrowserProperty = DependencyProperty.RegisterAttached(PropertyName_OpensInBrowser, typeof(bool), typeof(HyperlinkExtensions), new UIPropertyMetadata(false, OnOpensInBrowserChanged));
        /// <summary>
        /// Gets the value of the <see cref="OpensInBrowserProperty"/> property from a <see cref="Hyperlink"/> <see langword="class"/>.
        /// </summary>
        /// <param name="obj"><see cref="Hyperlink"/></param>
        /// <returns><see langword="true"/> when the hyperlink targets an external source; otherwise <see langword="false"/>.</returns>
        public static bool GetOpensInBrowser(DependencyObject obj) => (bool)obj.GetValue(OpensInBrowserProperty);
        /// <summary>
        /// Sets the value of the <see cref="OpensInBrowserProperty"/> property from a <see cref="Hyperlink"/> <see langword="class"/>.
        /// </summary>
        /// <param name="obj"><see cref="Hyperlink"/></param>
        /// <param name="value">The value to set the <see cref="OpensInBrowserProperty"/> to.</param>
        public static void SetOpensInBrowser(DependencyObject obj, bool value) => obj.SetValue(OpensInBrowserProperty, value);
        #endregion DependencyProperty

        #region EventHandlers
        private static void OnOpensInBrowserChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Hyperlink link)
            {
                if ((bool)e.NewValue)
                    link.RequestNavigate += Hyperlink_RequestNavigate;
                else
                    link.RequestNavigate -= Hyperlink_RequestNavigate;
            }
        }
        private static void Hyperlink_RequestNavigate(object? sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
                {
                    Verb = "open",
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Hyperlink_RequestNavigate)} failed due to an exception!", ex);
            }
        }
        #endregion EventHandlers
    }
}
