using System.ComponentModel;

namespace VolumeControl.Core.Controls
{
    public partial class TabSwitcher : UserControl
    {
        #region Constructors
        public TabSwitcher()
        {
            TabPages = new(this);

            using (Button b = new())
            {
                _tabStyle = b.FlatStyle;
                _tabAppearance = b.FlatAppearance;
            }

            InitializeComponent();
        }
        #endregion Constructors

        #region Members
        private FlatStyle _tabStyle;
        private FlatButtonAppearance _tabAppearance;
        private int _selectedIndex;
        #endregion Members

        #region Properties
        public TabPageList TabPages { get; }
        public object? SelectedItem
        {
            get => SelectedIndex >= 0 && SelectedIndex < TabPages.Count ? TabPages[SelectedIndex] : null;
            internal set
            {
                if (value is not TabPageEntry tpl)
                    return;

                int i = TabPages.IndexOf(tpl);
                if (i != -1)
                    SelectedIndex = i;
            }
        }
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (TabPages.Count == 0)
                    return;
                // disable the current page
                var current = SelectedItem;
                if (current is TabPageEntry currentSel)
                    currentSel.Page.Parent = PageParentDisabled;
                if (value < TabPages.Count)
                { // enable the new page
                    TabPageEntry sel = TabPages[_selectedIndex = value];
                    sel.Page.Parent = PageParentEnabled;
                }
                // notify
                NotifySelectedIndexChanged(EventArgs.Empty);
            }
        }
        public FlatStyle TabStyle
        {
            get => _tabStyle;
            set
            {
                _tabStyle = value;
                // update all tab buttons
                foreach (TabPageEntry page in TabPages)
                {
                    page.Header.FlatStyle = _tabStyle;
                }
            }
        }
        public FlatButtonAppearance TabAppearance
        {
            get => _tabAppearance;
            set
            {
                _tabAppearance = value;
                // update all tab buttons
                foreach (TabPageEntry page in TabPages)
                {
                    page.Header.FlatAppearance.BorderColor = _tabAppearance.BorderColor;
                    page.Header.FlatAppearance.MouseOverBackColor = _tabAppearance.MouseOverBackColor;
                    page.Header.FlatAppearance.MouseDownBackColor = _tabAppearance.MouseDownBackColor;
                    page.Header.FlatAppearance.CheckedBackColor = _tabAppearance.CheckedBackColor;
                }
            }
        }
        [DefaultValue(1)]
        public Padding TabMargin { get; set; }
        [DefaultValue(1)]
        public Padding PagePadding { get; set; }
        #endregion Properties

        #region Events
        public event EventHandler? SelectedIndexChanged = null!;
        private void NotifySelectedIndexChanged(EventArgs e) => SelectedIndexChanged?.Invoke(this, e);
        #endregion Events

        #region Methods
        #endregion Methods

        #region EventHandlers
        #endregion EventHandlers
    }
}
