using AudioAPI;
using Core.Enum;
using System.ComponentModel;

namespace Core
{
    public class AudioProcessAPI : INotifyPropertyChanged
    {
        public AudioProcessAPI(Dictionary<VolumeControlSubject, Dictionary<VolumeControlAction, HotkeyLib.KeyEventHandler?>> actions)
        {
            _procList = new();
            _selected = null;
            _selected_lock = false;
            _actions = actions;
        }

        #region Members
        private readonly AudioProcessList _procList;
        /// <summary>
        /// This maintains the previously selected target, in case it was removed from the list (terminated) and _selected_lock is true.
        /// </summary>
        private AudioProcess? _selected;
        private bool _selected_lock;
        private readonly Dictionary<VolumeControlSubject, Dictionary<VolumeControlAction, HotkeyLib.KeyEventHandler?>> _actions;
        #endregion Members

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public void InvokePropertyChanged(PropertyChangedEventArgs e)
            => PropertyChanged?.Invoke(this, e);
        public event EventHandler? SelectedProcessChanged;
        public void InvokeSelectedProcessChanged(EventArgs e)
            => SelectedProcessChanged?.Invoke(this, e);
        public event EventHandler? LockSelectionChanged;
        public void InvokeLockSelectionChanged(EventArgs e)
            => LockSelectionChanged?.Invoke(this, e);
        #endregion Events

        #region Properties
        public Dictionary<VolumeControlSubject, Dictionary<VolumeControlAction, HotkeyLib.KeyEventHandler?>> Actions
        {
            get => _actions;
        }
        public AudioProcessList ProcessList
        {
            get => _procList;
        }
        public bool LockSelection
        {
            get => _selected_lock;
            set
            {
                bool copy = _selected_lock;
                if ((_selected_lock = value) != copy) 
                    InvokeLockSelectionChanged(EventArgs.Empty);
            }
        }
        #endregion Properties

        #region Methods
        public AudioProcess GetSelectedProcess()
        {
            if (_selected == null)
                SelectNextProcess();
            return _selected!;
        }
        public void SetSelectedProcess(string name)
        {
            if (!_selected_lock)
            {
                _selected = ProcessList.FirstOrDefault(p => p.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase), null) ?? _selected;
                InvokeSelectedProcessChanged(EventArgs.Empty);
            }
        }
        public void SelectNextProcess()
        {
            if (!_selected_lock)
            {
                if (_selected != null)
                {
                    int i = ProcessList.IndexOf(_selected) + 1;
                    _selected = ProcessList[i % ProcessList.Count];
                }
                else _selected = ProcessList.First();
                InvokeSelectedProcessChanged(EventArgs.Empty);
            }
        }
        public void SelectPrevProcess()
        {
            if (!_selected_lock)
            {
                if (_selected != null)
                {
                    int i = ProcessList.IndexOf(_selected) - 1;
                    // extra handling because of C-style modulo not handling negative numbers
                    if (i < 0)
                        i = ProcessList.Count - 1;
                    _selected = ProcessList[i];
                }
                else _selected = ProcessList.Last();
                InvokeSelectedProcessChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Reloads the ProcessList.
        /// </summary>
        public void ReloadProcessList()
        {
            _procList.Reload();
        }

        public void SetHandler(VolumeControlSubject subject, VolumeControlAction action, HotkeyLib.KeyEventHandler handler)
            => _actions[subject][action] = handler;
        public HotkeyLib.KeyEventHandler? GetHandler(VolumeControlSubject subject, VolumeControlAction action)
            => _actions[subject][action];
        #endregion Methods
    }
}
