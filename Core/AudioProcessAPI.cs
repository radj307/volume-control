using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioAPI;
using AudioAPI.Forms;
using Core.Enum;
using HotkeyLib;

namespace Core
{
    public class AudioProcessAPI : INotifyPropertyChanged
    {
        public AudioProcessAPI(Dictionary<VolumeControlSubject, Dictionary<VolumeControlAction, HotkeyLib.KeyEventHandler?>> actions)
        {
            _procList = new();
            _selected = _procList.First();
            _selected_lock = false;
            _actions = actions;
        }

        #region Members
        private readonly AudioProcessList _procList;
        /// <summary>
        /// This maintains the previously selected target, in case it was removed from the list (terminated) and _selected_lock is true.
        /// </summary>
        private AudioProcess _selected;
        private bool _selected_lock;
        private readonly Dictionary<VolumeControlSubject, Dictionary<VolumeControlAction, HotkeyLib.KeyEventHandler?>> _actions;
        #endregion Members

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public void InvokePropertyChanged(PropertyChangedEventArgs e)
            => PropertyChanged?.Invoke(this, e);
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
            set => _selected_lock = value;
        }
        #endregion Properties

        #region Methods
        public void SetSelectedProcess(string name)
        {
            if (!_selected_lock)
                _selected = ProcessList.FirstOrDefault(p => p.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase), null) ?? _selected;
        }
        public void SetSelectedProcess(int index)
        {
            if (!_selected_lock)
            {
                int sz = _procList.Count;
                if (index < 0 || index >= sz)
                    throw new ArgumentOutOfRangeException($"Out-of-range error: {nameof(index)} (0-{sz})");
                _selected = _procList[index];
            }
        }

        public AudioProcess GetSelectedProcess()
            => _selected;

        public int GetSelectedProcessIndex()
            => _procList.IndexOf(_selected);

        public void SelectNextProcess()
        {
            int i = GetSelectedProcessIndex();
            if (i == -1)
                SetSelectedProcess(0);
            else 
                SetSelectedProcess(--i % _procList.Count);
        }
        public void SelectPrevProcess()
        {
            int i = GetSelectedProcessIndex();
            if (i == -1)
                SetSelectedProcess(0);
            else
                SetSelectedProcess(++i % _procList.Count);
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
