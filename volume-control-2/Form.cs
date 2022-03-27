using Core;
using System.Reflection;
using System.Windows.Forms;
using VolumeControl;

namespace volume_control_2
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            _api = new();
            InitializeComponent();
            bsAudioProcessAPI.DataSource = _api;

            

            Version currentVersion = typeof(Form).Assembly.GetName().Version!;
            if (Convert.ToBoolean(typeof(Form).Assembly.GetCustomAttribute<IsPreReleaseAttribute>()?.IsPreRelease))
                Label_Version.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}-pre{currentVersion.Revision}";
            else
                Label_Version.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}{(currentVersion.Revision >= 1 ? $"-{currentVersion.Revision}" : "")}";
        }

        /// <summary>
        /// The underlying controller object.
        /// </summary>
        private readonly AudioProcessAPI _api;
        /// <summary>
        /// The height of a single row in the mixer.
        /// </summary>
        private int _mixerListItemHeight = 0;

        public AudioProcessAPI API
        {
            get => _api;
        }

        private void SizeToFit()
        {
            if (_mixerListItemHeight == 0) // set the height of a list item
                _mixerListItemHeight = Mixer.Font.Height + 9;

            int padding = splitContainer1.Panel1.Height + splitContainer1.SplitterWidth + Mixer.ColumnHeadersHeight;
            if (FormBorderStyle != FormBorderStyle.None)
                padding += 40;

            int threeQuartersHeight = Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4);
            int totalFittingElements = threeQuartersHeight / _mixerListItemHeight;

            int totalCellHeight = _mixerListItemHeight * (Mixer.Rows.Count % totalFittingElements);

            // set the size of the form
            Size = new(Width, totalCellHeight + padding);
            this.UpdateBounds();
        }

        private void ReloadProcessList()
        {
            Mixer.ResetDataSource(bsAudioProcessAPI, API.ReloadProcessList);
        }

        /// <summary>
        /// Handles checkbox checked/unchecked events in the mixer datagrid.
        /// This allows the checkboxes to actually function.
        /// </summary>
        private void Mixer_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (Mixer.CurrentCell is DataGridViewCheckBoxCell)
                Mixer.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        /// <summary>
        /// This is triggered when rows are added from the list, including from the data binding source.
        /// </summary>
        private void Mixer_RowsAdded(object? sender, DataGridViewRowsAddedEventArgs e)
            => SizeToFit();
        /// <summary>
        /// This is triggered when rows are removed from the list, including from the data binding source.
        /// </summary>
        private void Mixer_RowsRemoved(object? sender, DataGridViewRowsRemovedEventArgs e)
            => SizeToFit();

        private void bReload_Click(object sender, EventArgs e)
            => ReloadProcessList();

        private void cbAutoReload_CheckedChanged(object sender, EventArgs e)
        {
            tAutoReload.Interval = Convert.ToInt32(nAutoReloadInterval.Value);
            tAutoReload.Enabled = nAutoReloadInterval.Enabled = cbAutoReload.Checked;
        }
        
        private void nAutoReloadInterval_ValueChanged(object sender, EventArgs e)
        {
            tAutoReload.Interval = Convert.ToInt32(nAutoReloadInterval.Value);
        }

        private void tAutoReload_Tick(object sender, EventArgs e)
        {
            ReloadProcessList();
        }
    }
}