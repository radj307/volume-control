using System.Collections;

namespace VolumeControl.Core.Keyboard
{
    public class HotkeyBindingList : ICollection<HotkeyBinding>, IEnumerable<HotkeyBinding>, IEnumerable, IList<HotkeyBinding>, IReadOnlyCollection<HotkeyBinding>, IReadOnlyList<HotkeyBinding>, ICollection, IList
    {
        public HotkeyBindingList() { _list = new(); }
        public HotkeyBindingList(List<HotkeyBinding> list) { _list = list; }

        public void BindHotkeyPressedEvents(AudioProcessAPI api)
        {
            foreach (HotkeyBinding hk in _list)
            {
                var handler = api.GetHandler(hk.Subject, hk.Action);
                if (handler != null)
                {
                    VC_Static.Log.Debug($"Successfully assigned action [{hk.Subject}:{hk.Action}] to hotkey '{hk.Name}'");
                    hk.Pressed += handler;
                }
                else VC_Static.Log.Error($"Couldn't find a valid action binding for hotkey: \'{hk.Name}\'");
            }
        }

        private readonly List<HotkeyBinding> _list;

        public int Count => ((ICollection<HotkeyBinding>)_list).Count;

        public bool IsReadOnly => ((ICollection<HotkeyBinding>)_list).IsReadOnly;

        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public object SyncRoot => ((ICollection)_list).SyncRoot;

        public bool IsFixedSize => ((IList)_list).IsFixedSize;

        object? IList.this[int index] { get => ((IList)_list)[index]; set => ((IList)_list)[index] = value; }
        HotkeyBinding IList<HotkeyBinding>.this[int index] { get => ((IList<HotkeyBinding>)_list)[index]; set => ((IList<HotkeyBinding>)_list)[index] = value; }

        public HotkeyBinding this[int index] => ((IReadOnlyList<HotkeyBinding>)_list)[index];

        public void Add(HotkeyBinding item)
        {
            ((ICollection<HotkeyBinding>)_list).Add(item);
        }

        public void Clear()
        {
            ((ICollection<HotkeyBinding>)_list).Clear();
        }

        public bool Contains(HotkeyBinding item)
        {
            return ((ICollection<HotkeyBinding>)_list).Contains(item);
        }

        public void CopyTo(HotkeyBinding[] array, int arrayIndex)
        {
            ((ICollection<HotkeyBinding>)_list).CopyTo(array, arrayIndex);
        }

        public bool Remove(HotkeyBinding item)
        {
            return ((ICollection<HotkeyBinding>)_list).Remove(item);
        }

        public IEnumerator<HotkeyBinding> GetEnumerator()
        {
            return ((IEnumerable<HotkeyBinding>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public int IndexOf(HotkeyBinding item)
        {
            return ((IList<HotkeyBinding>)_list).IndexOf(item);
        }

        public void Insert(int index, HotkeyBinding item)
        {
            ((IList<HotkeyBinding>)_list).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<HotkeyBinding>)_list).RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        public int Add(object? value)
        {
            return ((IList)_list).Add(value);
        }

        public bool Contains(object? value)
        {
            return ((IList)_list).Contains(value);
        }

        public int IndexOf(object? value)
        {
            return ((IList)_list).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)_list).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)_list).Remove(value);
        }
    }
}
