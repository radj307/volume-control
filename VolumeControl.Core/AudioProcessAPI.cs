using AudioAPI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Enum;

namespace VolumeControl.Core
{
    public class AudioProcessAPI
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
        /// <summary>
        /// Triggered when the selected process changes.
        /// </summary>
        public event EventHandler? SelectedProcessChanged;
        private void NotifySelectedProcessChanged(EventArgs e)
            => SelectedProcessChanged?.Invoke(this, e);

        /// <summary>
        /// Triggered when the LockSelection property changes.
        /// </summary>
        public event EventHandler? LockSelectionChanged;
        private void NotifyLockSelectionChanged(EventArgs e)
            => LockSelectionChanged?.Invoke(this, e);

        /// <summary>
        /// Triggered when the ProcessList is reloaded.
        /// </summary>
        public event EventHandler? ProcessListUpdated;
        private void NotifyProcessListUpdated(EventArgs e)
            => ProcessListUpdated?.Invoke(this, e);
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
                    NotifyLockSelectionChanged(EventArgs.Empty);
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
                _selected = ProcessList.FirstOrDefault(p => p != null && p.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase), null) ?? _selected;
                NotifySelectedProcessChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Select the process after the current selection.
        /// If no process is selected, the first process is used instead.
        /// </summary>
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
                NotifySelectedProcessChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Select the process before the current selection.
        /// If no process is selected, the last process is used instead.
        /// </summary>
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
                NotifySelectedProcessChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Reloads the ProcessList.
        /// </summary>
        public void ReloadProcessList()
        {
            _procList.Reload();
            NotifyProcessListUpdated(EventArgs.Empty);
        }

        public void SetHandler(VolumeControlSubject subject, VolumeControlAction action, HotkeyLib.KeyEventHandler handler)
            => _actions[subject][action] = handler;
        public HotkeyLib.KeyEventHandler? GetHandler(VolumeControlSubject subject, VolumeControlAction action)
            => _actions[subject][action];
        #endregion Methods
    }
}
