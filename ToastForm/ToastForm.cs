using ToastForm.Extensions;
using UIComposites;

namespace VolumeControl
{
    public partial class ToastForm : Form
    {
        #region Members

        private readonly CancelButtonHandler cancel = new();
        private readonly BindingSource listBindSource = new();
        private readonly int listItemHeight;
        private (int, int) positionPadding;

        #endregion Members

        #region Properties

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
                DisableListEventTriggers();
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
                EnableListEventTriggers();
            }
        }

        /// <summary>
        /// The color of the frame around the main list.
        /// </summary>
        public Color FrameColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }
        /// <summary>
        /// The background color of the main list
        /// </summary>
        public Color BackgroundColor
        {
            get => ListDisplay.BackColor;
            set => ListDisplay.BackColor = value;
        }

        /// <summary>
        /// Change the distance between the window and the edge of the screen
        /// Item1 = Width
        /// Item2 = Height
        /// </summary>
        public (int, int) PositionPadding
        {
            get => positionPadding;
            set => positionPadding = value;
        }

        public ListView.ListViewItemCollection Items => ListDisplay.Items;

        private void SetActiveStateImage(Image value)
        {
            StateImages.Images[1] = value;
        }

        private Image StateImageHigh => VolumeStateImageCache.Images[4];
        private Image StateImageMedium => VolumeStateImageCache.Images[3];
        private Image StateImageLow => VolumeStateImageCache.Images[2];
        private Image StateImageMuted => VolumeStateImageCache.Images[1];
        private Image StateImageNull => VolumeStateImageCache.Images[0];

        #endregion Properties

        #region Methods

        public void UpdateActiveStateImage(float volume, bool muted)
        {
            if (volume < 0f || volume > 1f)
                SetActiveStateImage(StateImageNull);
            else if (muted || volume.EqualsWithin(0f))
                SetActiveStateImage(StateImageMuted);
            else if (volume < 0.3f)
                SetActiveStateImage(StateImageLow);
            else if (volume < 0.6f)
                SetActiveStateImage(StateImageMedium);
            else
                SetActiveStateImage(StateImageHigh);
        }

        public void FlushItems() => Items.Clear();

        public void LoadItems(List<string> sessions)
        {
            foreach (string name in sessions)
            {
                Items.Add(name);
            }
            UpdatePosition();
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
            if (NotifyTimer.Enabled)
                NotifyTimer.Enabled = false;

            if (selectName.Length > 0)
                Selected = selectName;

            WindowState = FormWindowState.Normal;
            base.Show();

            if (enableTimeout)
            {
                NotifyTimer.Enabled = true;
            }
        }
        private void DisableListEventTriggers()
        {
            ListDisplay.ItemCheck -= ListDisplay_ItemCheck;
            ListDisplay.ItemActivate -= ListDisplay_ItemActivate;
        }
        private void EnableListEventTriggers()
        {
            ListDisplay.ItemCheck += ListDisplay_ItemCheck;
            ListDisplay.ItemActivate += ListDisplay_ItemActivate;
        }

        private void UpdatePosition()
        {
            SizeToFit();
            Position = new Point(
                Screen.PrimaryScreen.WorkingArea.Width - Width - positionPadding.Item1,
                Screen.PrimaryScreen.WorkingArea.Height - Height - positionPadding.Item2
            );
        }

        /// <summary>
        /// Calculates the amount of height needed to display all list items.
        /// </summary>
        private void SizeToFit()
        {
            int listSize = ListDisplay.Items.Count;
            if (listSize == 0)
                ++listSize;
            int height = listItemHeight * listSize;

            height += (DisplayPanel.Padding.Top + DisplayPanel.Padding.Bottom);

            if (height > Screen.PrimaryScreen.WorkingArea.Height)
                height = Screen.PrimaryScreen.WorkingArea.Height - 100;

            // set the size of the form
            Size = new(Width, height);
        }



        #endregion Methods

        #region Constructor

        public ToastForm()
        {
            onSelectionChanged = null!;
            InitializeComponent();

            Text = "Application Audio Sessions";

            cancel.Action += Hide;

            CancelButton = cancel;

            // Insert a temporary list item and draw the list to a bitmap image. (Item boundaries are set the first time it is drawn)
            ListDisplay.Items.Insert(0, "TEMP");
            ListDisplay.DrawToBitmap(new(100, 100), new Rectangle(0, 0, 100, 100));
            listItemHeight = ListDisplay.Items[0].GetBounds(ItemBoundsPortion.Entire).Height;
            ListDisplay.Items.Clear();

            // Then update the size & position of the window
            UpdatePosition();
        }

        #endregion Constructor

        #region FormEvents

        /// <summary>
        /// Triggered before an item is checked.
        /// This unchecks all items before the incoming changes.
        /// </summary>
        private void ListDisplay_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            DisableListEventTriggers(); //< Don't trigger event recursively (causes stack overflow)
            foreach (ListViewItem item in ListDisplay.Items)
            {
                if (item == null)
                    continue;
                item.Checked = false;
            }
            EnableListEventTriggers();
        }
        private void ListDisplay_ItemChecked(object sender, ItemCheckedEventArgs e) => OnSelectionChanged(e);

        private void ListDisplay_ItemActivate(object? sender, EventArgs e) => OnSelectionChanged(e);

        #endregion FormEvents

        #region CustomEvents

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
        protected virtual void OnSelectionChanged(EventArgs e) => onSelectionChanged?.Invoke(this, e);

        #endregion CustomEvents
    }
}
