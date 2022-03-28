using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public AudioProcessAPI()
        {
            _procList = new();
            _selected = _procList.First();
            _selected_lock = false;

            _actions = new()
            {
                {
                    VolumeControlSubject.VOLUME,
                    new()
                    {
                        { VolumeControlAction.INCREMENT, null },
                        { VolumeControlAction.DECREMENT, null },
                        { VolumeControlAction.TOGGLE, null },
                    }
                },
                {
                    VolumeControlSubject.MEDIA,
                    new()
                    {
                        { VolumeControlAction.NEXT, null },
                        { VolumeControlAction.PREV, null },
                        { VolumeControlAction.TOGGLE, null },
                    }
                },
                {
                    VolumeControlSubject.TARGET,
                    new()
                    {
                        { VolumeControlAction.NEXT, null },
                        { VolumeControlAction.PREV, null },
                        { VolumeControlAction.TOGGLE, null },
                    }
                },
            };
        }

        #region Members
        private AudioProcessList _procList;
        private AudioProcess _selected;
        private bool _selected_lock;
        private Dictionary<VolumeControlSubject, Dictionary<VolumeControlAction, HotkeyLib.KeyEventHandler?>> _actions;
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
            int i = 0;
            foreach (var proc in ProcessList)
            {
                if (proc.ProcessName == name || proc.DisplayName == name)
                {
                    _selected = proc;
                    break;
                }
                ++i;
            }
        }

        public AudioProcess GetSelectedProcess()
            => _selected;

        /// <summary>
        /// Reloads the ProcessList.
        /// </summary>
        public void ReloadProcessList()
        {
            //int index = _procList.IndexOf(_selected);
            _procList.Reload();
            /*if (!_procList.Contains(_selected))
            {
                bool validIndex = index != -1;
                if (_selected_lock)
                {
                    _procList.Add(_selected);
                    _procList = (AudioProcessList)_procList.OrderBy(i => i.ProcessName);
                }
                else if (validIndex && index - 1 >= 0 && index - 1 < _procList.Count)
                    _selected = _procList[index - 1];
                else if (validIndex && index + 1 < _procList.Count)
                    _selected = _procList[index + 1];
                else // invalid index:
                    _selected = _procList.First();
            }*/
        }

        public void SetHandler(VolumeControlSubject subject, VolumeControlAction action, HotkeyLib.KeyEventHandler handler)
        {
            _actions[subject][action] = handler;
        }
        public HotkeyLib.KeyEventHandler? GetHandler(VolumeControlSubject subject, VolumeControlAction action)
        {
            return _actions[subject][action];
        }
        #endregion Methods
    }
}
