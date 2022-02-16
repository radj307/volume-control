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
                    if (entry == null)
                        continue;
                    if (entry.Checked)
                    {
                        return entry.Text;
                    }
                }
                return string.Empty;
            }
            set
            {
                DisableEventTriggers();
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
                EnableEventTriggers();
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
                    NotifyTimer.Enabled = false;
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

        private void DisableEventTriggers()
        {
            ListDisplay.ItemCheck -= ListDisplay_ItemCheck;
            ListDisplay.ItemActivate -= ListDisplay_ItemActivate;
        }
        private void EnableEventTriggers()
        {
            ListDisplay.ItemCheck += ListDisplay_ItemCheck;
            ListDisplay.ItemActivate += ListDisplay_ItemActivate;
        }

        /// <summary>
        /// Triggered before an item is checked.
        /// This unchecks all items before the incoming changes.
        /// </summary>
        private void ListDisplay_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            DisableEventTriggers(); //< Don't trigger event recursively (causes stack overflow)
            foreach (ListViewItem item in ListDisplay.Items)
            {
                if (item == null)
                    continue;
                item.Checked = false;
            }
            EnableEventTriggers();
        }
        private void ListDisplay_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            OnSelectionChanged(e);
        }

        private void ListDisplay_ItemActivate(object? sender, EventArgs e)
        {
            OnSelectionChanged(e);
        }

        #region Events

        /// <summary>
        /// Custom Event
        /// Triggers when any key modifiers are changed.
        /// </summary>
        private EventHandler onSelectionChanged;
        public event EventHandler SelectionChanged
        {
            add
            {
                onSelectionChanged += value;
            }
            remove
            {
#               pragma warning disable CS8601 // Possible null reference assignment.
                onSelectionChanged -= value;
#               pragma warning restore CS8601 // Possible null reference assignment.
            }
        }
        /// <summary>
        /// Trigger a ModifierChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            onSelectionChanged?.Invoke(this, e);
        }

        #endregion Events
    }
}
