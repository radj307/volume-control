using VolumeControl.Core;
using VolumeControl.Core.Controls.Forms;

namespace VolumeControl
{
    /// <summary>
    /// This object contains the top-level logic and handles the creation and deletion of all forms used by Volume Control.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <code language="cs">
    /// VCApplicationContext appContext = new();
    /// Application.Run(appContext);
    /// </code>
    /// </remarks>
    internal class VCApplicationContext : ApplicationContext
    {
        #region Constructors
        public VCApplicationContext()
        {
            // Initialize forms
            hkeditForm = new HotkeyEditorForm();
            toastForm = new ToastForm();
            indicatorForm = new VolumeIndicatorForm();
            vcForm = new MainForm(hkeditForm, toastForm, indicatorForm);

            // Initialize form counter
            openForms = 4;

            // debug form
            if (Properties.Settings.Default.SHOW_DEBUG_FORM)
            {
                debugForm = new DebugForm(vcForm)
                {
                    SelectedObject = vcForm
                };
                openForms++;
                // add event handlers
                debugForm.FormClosing += (s, e) =>
                {
                    vcForm.Close();
                    e.Cancel = false;
                };
                debugForm.FormClosed += HandleFormClosed!;
                // show form
                debugForm.Show();
            }

            // if the toastform & indicator are shown in the same corner, increase the amount of padding for the toastform so it doesn't overlap
            if (toastForm.DisplayCorner == indicatorForm.DisplayCorner)
                toastForm.DisplayPadding = new(toastForm.DisplayPadding.Width + indicatorForm.Size.Width + indicatorForm.DisplayPadding.Width, toastForm.DisplayPadding.Height);//.Width += ;

            // Set owners
            hkeditForm.Owner = vcForm;
            toastForm.Owner = vcForm;
            indicatorForm.Owner = vcForm;

            // Set event callbacks
            hkeditForm.FormClosed += HandleFormClosed!;
            toastForm.FormClosed += HandleFormClosed!;
            indicatorForm.FormClosed += HandleFormClosed!;
            vcForm.FormClosed += HandleFormClosed!;

            if (!Properties.Settings.Default.StartMinimized)
            {
                vcForm.Show();
            }
        }
        #endregion Constructors

        #region Fields
        private readonly HotkeyEditorForm hkeditForm;
        private readonly ToastForm toastForm;
        private readonly VolumeIndicatorForm indicatorForm;
        private readonly MainForm vcForm;
        private readonly DebugForm? debugForm = null;
        private int openForms;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Handles <see cref="System.Windows.Forms.Form.FormClosed"/> events.<br/>
        /// This kills the associated thread and decrements the <see cref="openForms"/> counter.
        /// </summary>
        /// <remarks>These events are only triggered once the <see cref="System.Windows.Forms.Form.FormClosing"/> event has returned <b>without setting Cancel to true.</b></remarks>
        /// <param name="sender">The form that sent the event.</param>
        /// <param name="e">The event arguments associated with this event.</param>
        private void HandleFormClosed(object sender, FormClosedEventArgs e)
        {
            // if this event was triggered for the main form, do some extra cleanup
            if (sender is MainForm form)
            { // hide the tray icon
                if (form.TrayIcon.Container != null)
                {
                    foreach (object? item in form.TrayIcon.Container.Components)
                    {
                        if (item != null && !item.Equals(form.TrayIcon) && item is IDisposable disposableItem)
                            disposableItem.Dispose();
                    }
                }
                form.TrayIcon.Visible = false;
                form.TrayIcon.Icon = null;
                form.TrayIcon.Dispose();
                VC_Static.Log.Debug("Finished cleaning up the tray icon.");
            }
            VC_Static.Log.Info($"Closing Form:  '{sender.GetType().FullName}'");
            if (Interlocked.Decrement(ref openForms) == 0)
                ExitThread();
        }
        #endregion Methods
    }
}