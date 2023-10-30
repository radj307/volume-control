using Microsoft.Xaml.Behaviors;
using System;
using System.Diagnostics;
using System.Windows.Documents;
using VolumeControl.Log;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// <see cref="Hyperlink"/> behavior that opens the link in the default browser.
    /// </summary>
    public sealed class HyperlinkOpenInBrowserBehavior : Behavior<Hyperlink>
    {
        #region Behavior Method Overrides
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            AssociatedObject.RequestNavigate += this.AssociatedObject_RequestNavigate;

            base.OnAttached();
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.RequestNavigate -= this.AssociatedObject_RequestNavigate;

            base.OnDetaching();
        }
        #endregion Behavior Method Overrides

        #region EventHandlers

        #region AssociatedObject
        private void AssociatedObject_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var uri = e.Uri.AbsoluteUri;
            try
            {
                Process.Start(new ProcessStartInfo(uri)
                {
                    Verb = "open",
                    UseShellExecute = true,
                })?.Dispose();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                FLog.Error($"Failed to open \"{uri}\" due to an exception:", ex);
            }
        }
        #endregion AssociatedObject

        #endregion EventHandlers
    }
}
