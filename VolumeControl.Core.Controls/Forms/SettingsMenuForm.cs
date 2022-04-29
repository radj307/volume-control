using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VolumeControl.Core.Controls.Forms
{
    public partial class SettingsMenuForm : Form
    {
        public SettingsMenuForm()
        {
            InitializeComponent();

            // fix the cancel button
            CancelButton = vbCancel;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the number of milliseconds to wait between opacity changes when the form is fading in/out.
        /// </summary>
        [DefaultValue(20)]
        [RegularExpression("[1-9][0-9]+", ErrorMessage = "Enter any number greater than 0.")]
        public uint FadeInterval { get; set; }
        /// <summary>
        /// Gets or sets the amount of opacity deducted from, or added to, the form's opacity every <see cref="FadeInterval"/> ms.
        /// </summary>
        [DefaultValue(0.1)]
        [Range(0.000001, 1.0)]
        public double FadeStep { get; set; }
        #endregion Properties

        #region Methods
        private void FadeOut()
        {
            double opacity = Opacity;
            // reduce the opacity
            while (Opacity > 0.0)
            {
                Opacity -= FadeStep;
                Thread.Sleep((int)FadeInterval);
            }
            // hide the form
            Minimize();
            // reset the opacity for next time
            Opacity = opacity;
        }
        private void FadeIn()
        {
            double opacity = Opacity;
            Opacity = 0.0;

            while (Opacity < opacity)
            {
                Opacity += FadeStep;
                Thread.Sleep((int)FadeInterval);
            }

            Opacity = opacity;
        }
        private void Minimize()
        {
            WindowState = FormWindowState.Minimized;
        }
        private void UnMinimize()
        {
            WindowState = FormWindowState.Normal;
        }
        public new void Hide()
        {
            FadeOut();
        }
        public new void Show()
        {
            FadeIn();
        }
        #endregion Methods

        #region EventHandlers
        private void Minimize(object sender, EventArgs e) => Minimize();
        private void UnMinimize(object sender, EventArgs e) => UnMinimize();
        private void HandleFormResize(object sender, EventArgs e)
        {
            if (!(Visible = WindowState != FormWindowState.Minimized))
            {
                // put stuff that you want to happen when the form is not visible anymore
            }
        }
        private void HandleFormHelpButton(object sender, CancelEventArgs e)
        {

        }
        private void HandleFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        private void TabControlDrawItem(object sender, DrawItemEventArgs e)
        {
            if (sender is not Panel page)
                return;

            Graphics g = e.Graphics;
            var rect = e.Bounds;

            g.FillRectangle(new SolidBrush(page.BackColor), rect);
        }
        #endregion EventHandlers

        private void button1_Click(object sender, EventArgs e)
        {
            Panel page = new();
            page.BackColor = Color.FromArgb(Random.Shared.Next(255), Random.Shared.Next(255), Random.Shared.Next(255));
            Label l = new();
            l.Parent = page;
            l.Location = new(3, 3);
            byte[] arr = new byte[10];
            Random.Shared.NextBytes(arr);
            foreach (byte rand in arr)
            {
                l.Text += (char)rand;
            }
            Button b = new();
            b.Text = $"Tab {tabSwitcher1.TabPages.Count}";
            tabSwitcher1.TabPages.Add(new(b, page));
        }
    }
}
