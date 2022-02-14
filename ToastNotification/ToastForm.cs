using UIComposites;

namespace VolumeControl
{
    public partial class ToastForm : Form
    {
        private readonly CancelButtonHandler cancel = new();
        private readonly BindingSource listBindSource = new();
        private bool timeout_enabled = false;

        public object DataSource
        {
            get => listBindSource.DataSource;
            set => listBindSource.DataSource = value;
        }

        public int Timeout
        {
            get => TimeoutTimer.Interval;
            set => TimeoutTimer.Interval = value;
        }

        public bool TimeoutEnabled
        {
            get => timeout_enabled;
            set => timeout_enabled = value;
        }

        public string Selected
        {
            get
            {
                foreach (ListViewItem entry in ListDisplay.SelectedItems)
                {
                    if (entry.Selected)
                    {
                        return entry.Text;
                    }
                }
                return string.Empty;
            }
            set
            {
                foreach (ListViewItem entry in ListDisplay.SelectedItems)
                {
                    if (entry.Text.Equals(value, StringComparison.OrdinalIgnoreCase))
                        entry.Selected = true;
                    else
                        entry.Selected = false;
                }
            }
        }

        public ListView.ListViewItemCollection Items
        {
            get => ListDisplay.Items;
        }

        public void FlushItems()
        {
            Items.Clear();
        }

        public void LoadItems(List<string> sessions)
        {
            foreach (string name in sessions)
            {
                Items.Add(name);
            }
        }

        public void SetTitle(string name)
        {
            Text = name;
        }

        /// <summary>
        /// Hide the toast notification form
        /// </summary>
        public new void Hide()
        {
            TimeoutTimer.Stop();
            TimeoutTimer.Enabled = false;
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// Show the toast notification form
        /// </summary>
        public new void Show()
        {
            if (TimeoutTimer.Enabled)
                TimeoutTimer.Stop();
            WindowState = FormWindowState.Normal;
            base.Show();
            if (timeout_enabled)
            {
                TimeoutTimer.Enabled = true;
                TimeoutTimer.Start();
            }
        }

        public ToastForm()
        {
            InitializeComponent();
            Text = "Application Audio Sessions";

            TimeoutTimer.Tick += delegate { Hide(); };
            cancel.Action += delegate { Hide(); };

            CancelButton = cancel;
        }
    }
}
