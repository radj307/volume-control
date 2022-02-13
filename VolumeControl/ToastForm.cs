namespace VolumeControl
{
    public partial class ToastForm : Form
    {
        private readonly System.Timers.Timer timer = new();
        private bool enabled = true;
        private readonly CancelButtonHandler cancel = new();
        private readonly BindingSource listBindSource = new();

        public object DataSource
        {
            get => listBindSource.DataSource;
            set => listBindSource.DataSource = value;
        }

        public bool ToastEnabled
        {
            get => enabled;
            set => enabled = value;
        }
        public double Timeout
        {
            get => timer.Interval;
            set => timer.Interval = value;
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
            timer.Stop();
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// Show the toast notification form
        /// </summary>
        public new void Show()
        {
            if (enabled)
            {
                WindowState = FormWindowState.Normal;
                base.Show();
                timer.Start();
            }
        }
        public void Show(Point origin)
        {
            if (enabled)
            {
                WindowState = FormWindowState.Normal;
                Location = origin;
                base.Show();
                timer.Start();
            }
        }

        public ToastForm()
        {
            InitializeComponent();
            Text = "Application Audio Sessions";

            timer.Elapsed += delegate { Hide(); };
            cancel.Action += delegate { Hide(); };

            CancelButton = cancel;
        }
    }
}
