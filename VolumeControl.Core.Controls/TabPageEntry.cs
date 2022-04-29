using System.Collections;

namespace VolumeControl.Core.Controls
{
    public class TabPageEntry
    {
        public TabPageEntry((Button, Panel) pr)
        {
            Header = pr.Item1;
            Page = pr.Item2;
        }
        public TabPageEntry(Button header, Panel page)
        {
            Header = header;
            Page = page;
        }
        public TabPageEntry() : this(new(), new()) { }

        public Button Header { get; set; }
        public Panel Page { get; set; }
    }
    public class TabPageList : ICollection<TabPageEntry>, IEnumerable<TabPageEntry>, IEnumerable, IList<TabPageEntry>, IReadOnlyCollection<TabPageEntry>, IReadOnlyList<TabPageEntry>, ICollection, IList
    {
        public TabPageList(TabSwitcher tabSwitcherInst)
        {
            switcher = tabSwitcherInst;
            List = new();
        }

        private readonly TabSwitcher switcher;

        public TabPageEntry this[int index] { get => ((IList<TabPageEntry>)List)[index]; set => ((IList<TabPageEntry>)List)[index] = value; }
        object? IList.this[int index] { get => ((IList)List)[index]; set => ((IList)List)[index] = value; }

        public List<TabPageEntry> List { get; }

        public int Count => ((ICollection<TabPageEntry>)List).Count;

        public bool IsReadOnly => ((ICollection<TabPageEntry>)List).IsReadOnly;

        public bool IsSynchronized => ((ICollection)List).IsSynchronized;

        public object SyncRoot => ((ICollection)List).SyncRoot;

        public bool IsFixedSize => ((IList)List).IsFixedSize;

        /// <summary>
        /// This literally just calls the generic add function below because fuck the amount of boilerplate code C# requires for literally anything simple.<br/>
        /// who the fuck was it that decided that boilerplate over substance is anything other than lunacy
        /// </summary>
        public void Add(TabPageEntry item)
            => Add((object)item);
        public int Add(object? value)
        {
            if (value is not TabPageEntry entry)
                throw new ArgumentNullException(nameof(value));

            entry.Header.Parent = switcher.HeaderParent;
            entry.Header.Margin = switcher.TabMargin;
            entry.Header.Padding = new(0);
            entry.Header.FlatStyle = switcher.TabStyle;
            entry.Header.FlatAppearance.BorderColor = switcher.TabAppearance.BorderColor;
            entry.Header.FlatAppearance.BorderSize = switcher.TabAppearance.BorderSize;
            entry.Header.FlatAppearance.MouseOverBackColor = switcher.TabAppearance.MouseOverBackColor;
            entry.Header.FlatAppearance.MouseDownBackColor = switcher.TabAppearance.MouseDownBackColor;
            entry.Header.Dock = DockStyle.Fill;
            entry.Header.Click += delegate
            {
                switcher.SelectedItem = entry;
            };

            entry.Page.Parent = (Count == 0) ? switcher.PageParentEnabled : switcher.PageParentDisabled;
            entry.Page.Padding = switcher.PagePadding;
            entry.Page.Dock = DockStyle.Fill;

            return ((IList)List).Add(value);
        }
        public void Clear() => ((ICollection<TabPageEntry>)List).Clear();
        public bool Contains(TabPageEntry item) => ((ICollection<TabPageEntry>)List).Contains(item);
        public bool Contains(object? value) => ((IList)List).Contains(value);
        public void CopyTo(TabPageEntry[] array, int arrayIndex) => ((ICollection<TabPageEntry>)List).CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)List).CopyTo(array, index);
        public IEnumerator<TabPageEntry> GetEnumerator() => ((IEnumerable<TabPageEntry>)List).GetEnumerator();
        public int IndexOf(TabPageEntry item) => ((IList<TabPageEntry>)List).IndexOf(item);
        public int IndexOf(object? value) => ((IList)List).IndexOf(value);
        public void Insert(int index, TabPageEntry item) => ((IList<TabPageEntry>)List).Insert(index, item);
        public void Insert(int index, object? value) => ((IList)List).Insert(index, value);
        public bool Remove(TabPageEntry item) => ((ICollection<TabPageEntry>)List).Remove(item);
        public void Remove(object? value) => ((IList)List).Remove(value);
        public void RemoveAt(int index) => ((IList<TabPageEntry>)List).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)List).GetEnumerator();
    }
}
