using UIComposites;

namespace VolumeControl
{
    public partial class ToastForm : Form
    {
        private readonly CancelButtonHandler cancel = new();
        private readonly BindingSource listBindSource = new();

        public object DataSource
        {
            get => listBindSource.DataSource;
            set => listBindSource.DataSource = value;
        }

        public bool TimeoutEnabled
        {
            get => NotifyTimer.Enabled;
            set => NotifyTimer.Enabled = value;
        }

        public int Timeout
        {
            get => NotifyTimer.Interval;
            set => NotifyTimer.Interval = value;
        }

        public Point Position
        {
            get => Location;
            set => Location = value;
        }
        public int PositionX
        {
            get => Position.X;
            set => Position = new Point(value, Position.Y);
        }
        public int PositionY
        {
            get => Position.Y;
            set => Position = new Point(Position.X, value);
        }

        public string Selected
        {
            get
            {
                foreach (ListViewItem entry in ListDisplay.Items)
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
                foreach (ListViewItem entry in ListDisplay.Items)
                {
                    if (entry.Text.Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        entry.Checked = true;
                        entry.Selected = true;
                    }
                    else
                    {
                        entry.Checked = false;
                        entry.Selected = false;
                    }
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

        /// <summary>
        /// Hide the toast notification form
        /// </summary>
        public void Hide(object? sender, EventArgs e)
        {
            NotifyTimer.Stop();
            NotifyTimer.Enabled = false;
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// Show the toast notification form
        /// </summary>
        public void Show(bool enableTimeout = false, string selectName = "")
        {
            if (selectName.Length > 0)
                Selected = selectName;
            WindowState = FormWindowState.Normal;
            base.Show();
            if (enableTimeout)
            {
                if (NotifyTimer.Enabled)
                {
                    NotifyTimer.Enabled = false;
                }
                NotifyTimer.Enabled = true;
                NotifyTimer.Start();
            }
        }

        public ToastForm()
        {
            InitializeComponent();
            Text = "Application Audio Sessions";
            Position = new Point(
                Screen.PrimaryScreen.WorkingArea.Width - Width - 10,
                Screen.PrimaryScreen.WorkingArea.Height - Height - 10
            );

            NotifyTimer.Tick += Hide;
            cancel.Action += Hide;

            CancelButton = cancel;
        }
    }
}
