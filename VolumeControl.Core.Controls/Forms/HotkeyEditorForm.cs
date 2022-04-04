﻿using VolumeControl.Core.Keyboard;

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
            => VC_Static.Log.WriteExceptionError(e.Exception);
        #endregion ControlEventHandlers

        private void HotkeyEditorForm_Load(object sender, EventArgs e)
            => ResumeSizeToFit(true);
    }
}