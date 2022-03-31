using VolumeControl.Core.Keyboard;

namespace VolumeControl.Core.Controls
{
    public partial class HotkeyEditorForm : Form
    {
        public HotkeyEditorForm()
        {
            _hotkeys = new();
            InitializeComponent();
            bsHotkeyBindingList.DataSource = _hotkeys;
            bsKeysList.DataSource = new ValidKeys();
        }
        /// <summary>
        /// Called before the form closes.
        /// This is basically a cancellable finalizer.
        /// </summary>
        private void HotkeyEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // cancel the close event to prevent this form from being deleted when the program is still running
            e.Cancel = true;
            // hide this form instead
            Hide();
            // save hotkeys
            VC_Static.SaveSettings();
        }

        private readonly HotkeyBindingList _hotkeys;
        private int _dgvListItemHeight = 0;

        private void SizeToFit()
        {
            if (_dgvListItemHeight == 0) // set the height of a list item
                _dgvListItemHeight = dgv.Font.Height + 9;

            int sizeY = dgvPanel.Padding.Top + dgvPanel.Padding.Bottom + dgv.ColumnHeadersHeight;
            if (FormBorderStyle != FormBorderStyle.None)
                sizeY += 40;

            int threeQuartersHeight = Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4);
            int totalFittingElements = threeQuartersHeight / _dgvListItemHeight;
            sizeY += _dgvListItemHeight * (dgv.Rows.Count % totalFittingElements);

            // set the size of the form
            Size = new(Width, sizeY);
            this.UpdateBounds();
        }

        public object SetDataSource(object src)
            => dgv.DataSource = src;

        public new void Hide()
        {
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        public new void Show()
        {
            SuspendLayout();
            SizeToFit();
            Refresh();
            WindowState = FormWindowState.Normal;
            Visible = true;
            ResumeLayout();
        }
        public void Toggle()
        {
            if (Visible)
                Hide();
            else
                Show();
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentCell is DataGridViewCheckBoxCell)
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }


        private void bCancel_Click(object sender, EventArgs e)
            => Hide();

        private void dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
            => SizeToFit();

        private void dgv_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
            => SizeToFit();

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
            => VC_Static.Log.WriteExceptionError(e.Exception);
    }
}
