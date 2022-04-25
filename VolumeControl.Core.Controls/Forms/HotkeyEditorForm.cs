using VolumeControl.Core.Keyboard;

namespace VolumeControl.Core.Controls
{
    public partial class HotkeyEditorForm : Form
    {
        #region Constructors
        public HotkeyEditorForm()
        {
            InitializeComponent();
            bsKeysList.DataSource = new ValidKeys();
        }
        #endregion Constructors

        #region Finalizers
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
        }
        #endregion Finalizers

        #region Members
        private int _dgvListItemHeight = 0;
        private bool _allowAutoSize = false;
        #endregion Members

        #region Properties
        public object DataSource
        {
            get => dgv.DataSource;
            set => dgv.DataSource = value;
        }
        #endregion Properties

        #region Methods
        public void SuspendSizeToFit()
            => _allowAutoSize = false;
        public void ResumeSizeToFit(bool trigger = false)
        {
            _allowAutoSize = true;
            if (trigger)
                SizeToFit();
        }
        private void SizeToFit()
        {
            if (!_allowAutoSize)
                return;

            if (_dgvListItemHeight == 0) // set the height of a list item
                _dgvListItemHeight = dgv.Font.Height + 9;

            int sizeY = dgv.ColumnHeadersHeight;
            if (FormBorderStyle != FormBorderStyle.None)
                sizeY += 40;

            int threeQuartersHeight = Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4);
            int totalFittingElements = threeQuartersHeight / _dgvListItemHeight;
            sizeY += _dgvListItemHeight * (dgv.Rows.Count % totalFittingElements);

            // set the size of the form
            Size = new(Width, sizeY);
            this.UpdateBounds();
        }

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
        #endregion Methods

        #region ControlEventHandlers
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
            => VC_Static.Log.ErrorException(e.Exception);
        private void HotkeyEditorForm_Load(object sender, EventArgs e)
            => ResumeSizeToFit(true);
        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == colEnabled.Index || e.ColumnIndex == colAlt.Index || e.ColumnIndex == colCtrl.Index || e.ColumnIndex == colShift.Index || e.ColumnIndex == colWin.Index)
                {
                    e.Handled = true;
                    Graphics g = e.Graphics;
                    var rect = e.CellBounds;
                    int bottom = rect.Y + rect.Height - 1;
                    rect.Size = new(rect.Width, rect.Height - 1);
                    // draw background
                    g.FillRectangle(new SolidBrush(e.CellStyle.BackColor), rect);
                    // draw divider
                    g.DrawLine(new Pen(new SolidBrush(dgv.GridColor), 1f), new(rect.X, bottom), new Point(rect.X + rect.Width, bottom));

                    int boxSize = 13;
                    var box = new Rectangle(rect.Location.X + (rect.Width / 2 - boxSize / 2), rect.Location.Y + (rect.Height / 2 - boxSize / 2), boxSize, boxSize);
                    var b = new SolidBrush(Color.FromArgb(200, 200, 200));
                    g.DrawRectangle(new Pen(b, 1f), box);
                    if (Convert.ToBoolean(e.Value))
                    {
                        g.FillRectangle(b, new Rectangle(box.X + 3, box.Y + 3, box.Width - 5, box.Height - 5));
                    }
                }
            }
        }
        private void HotkeyEditorForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
            => MessageBox.Show(
            this,
            "Each row corresponds to a different hotkey. The 'Name' column indicates the hotkey's function.\n\nThe dropdown boxes under the 'Key' header select the primary key (or button) for a hotkey.\n\nThe 'Alt', 'Ctrl', 'Shift', & 'Win' checkboxes may be used to set modifier keys.\nIf your keyboard has multiple modifier keys (left/right on most keyboards), you can use either one.\n\nTo enable a hotkey, check the 'Active' box next to its name.\nHotkeys can only be enabled if they have a valid key set. (All keys except 'None' are considered valid.)",
            "Hotkey Editor Usage",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1
        );
        private void vbCancel_Click(object sender, EventArgs e)
            => Close();
        #endregion ControlEventHandlers
    }
}
