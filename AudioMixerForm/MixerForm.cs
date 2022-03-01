using AudioAPI;

namespace AudioMixer
{
    public partial class MixerForm : Form
    {
        public MixerForm(bool autoReloadEnabled)
        {
            InitializeComponent();

            audioProcessList = new();
            gridBindingSource.DataSource = audioProcessList;

            listItemHeight = grid.Font.Height + 9;
        }
        public MixerForm() : this(false) { }

        private readonly int listItemHeight;
        private readonly AudioProcessList audioProcessList;

        public Point Position
        {
            get => Location;
            set => Location = value;
        }
        public int PositionX
        {
            get => Location.X;
            set => Position = new Point(value, PositionY);
        }
        public int PositionY
        {
            get => Location.Y;
            set => Position = new Point(PositionX, value);
        }

        /// <summary>
        /// Enable or disable the automatic reload of the process list.
        /// </summary>
        public bool AutoReload
        {
            get => ReloadTimer.Enabled;
            set => ReloadTimer.Enabled = value;
        }

        /// <summary>
        /// Set the automatic reload timer's tick interval.
        /// </summary>
        public int AutoReloadInterval
        {
            get => ReloadTimer.Interval;
            set => ReloadTimer.Interval = value;
        }

        /// <summary>
        /// Calculates the amount of height needed to display all list items.
        /// </summary>
        private void SizeToFit()
        {
            int padding = panel.Padding.Top + panel.Padding.Bottom + grid.ColumnHeadersHeight;

            if (FormBorderStyle != FormBorderStyle.None)
                padding += 40;

            int totalCellHeight = listItemHeight * (grid.RowCount % (Screen.PrimaryScreen.WorkingArea.Height / listItemHeight));

            // set the size of the form
            Size = new(Width, totalCellHeight + padding);
        }

        private void ReloadTimer_Tick(object sender, EventArgs e)
        {
            audioProcessList.Reload();
            gridBindingSource.DataSource = audioProcessList;
            SizeToFit();
            Refresh();
        }

        /// <summary>
        /// Handles CheckedChanged events for checkboxes in the data grid.
        /// </summary>
        private void grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (grid.CurrentCell is DataGridViewCheckBoxCell)
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (grid.CurrentCell is DataGridViewTextBoxCell cell && cell.ColumnIndex == dgvColumn_Volume.Index)
            {
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        public event EventHandler SelectionChanged
        {
            add => grid.SelectionChanged += value;
            remove => grid.SelectionChanged -= value;
        }

        private void grid_DataSourceChanged(object sender, EventArgs e)
        {
            SizeToFit();
        }
    }
}
