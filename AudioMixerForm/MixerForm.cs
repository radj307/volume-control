using AudioAPI;
using AudioAPI.Forms;

namespace AudioMixer
{
    public partial class MixerForm : Form
    {
        #region Constructors

        public MixerForm(bool autoReloadEnabled)
        {
            InitializeComponent();

            ReloadTimer.Enabled = autoReloadEnabled;

            audioProcessList = new();
            gridBindingSource.DataSource = audioProcessList;

            listItemHeight = grid.Font.Height + 9;

            int threeQuartersHeight = Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4);

            totalFittingElements = threeQuartersHeight / listItemHeight;

            SizeToFit();
        }

        public MixerForm() : this(false) { }

        #endregion Constructors

        #region Members

        private readonly int listItemHeight, totalFittingElements;
        private readonly AudioProcessList audioProcessList;

        #endregion Members

        #region Properties

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

        public DataGridViewCell CurrentCell
        {
            get => grid.CurrentCell;
            set => grid.CurrentCell = value;
        }

        public DataGridViewRow CurrentRow
        {
            get => grid.CurrentRow;
        }

        public int CurrentCellRowIndex
        {
            get => CurrentRow.Index;
            set
            {
                for (int i = 0; i < grid.Rows.Count; ++i)
                {
                    grid.Rows[i].Selected = (i == value);
                }
            }
        }

        public string CurrentCellProcessName
        {
            get => CurrentRow.Cells[dgvColumn_ProcessName.Index]?.Value?.ToString() ?? "";
            set
            {
                for (int i = 0; i < grid.Rows.Count; ++i)
                {
                    grid.Rows[i].Selected = (grid.Rows[i].Cells[dgvColumn_ProcessName.Index].Value.ToString() == value);
                }
            }
        }

        public float VolumeStepSize
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Calculates the amount of height needed to display all list items.
        /// </summary>
        private void SizeToFit()
        {
            int padding = panel.Padding.Top + panel.Padding.Bottom + grid.ColumnHeadersHeight;

            if (FormBorderStyle != FormBorderStyle.None)
                padding += 40;

            int totalCellHeight = listItemHeight * (grid.RowCount % totalFittingElements);

            // set the size of the form
            Size = new(Width, totalCellHeight + padding);
        }

        #endregion Methods

        #region FormMethods

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

        private void grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
            => SizeToFit();

        private void grid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
            => SizeToFit();

        #endregion FormMethods

        #region Events

        public event EventHandler SelectionChanged
        {
            add => grid.SelectionChanged += value;
            remove => grid.SelectionChanged -= value;
        }

        #endregion Events
    }
}
