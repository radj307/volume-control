using AudioAPI;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Audio;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Events;
using VolumeControl.Log;

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

            _allowReload = false;
            _reloadTimer = new(10000)
            {
                AutoReset = false
            };
            _reloadTimer.Elapsed += ReloadTimerElapsed!;
            _reloadTimer.Start();

            ReloadOnHotkey = true;
        }

        #region Members
        private readonly AudioProcessList _procList;
        /// <summary>
        /// This maintains the previously selected target, in case it was removed from the list (terminated) and _selected_lock is true.
        /// </summary>
        private IAudioProcess? _selected;
        private bool _selected_lock;
        private bool _allowReload;
        /// <summary>
        /// Timer that prevents rapid successive key presses from reloading the list every time.
        /// </summary>
        private readonly System.Timers.Timer _reloadTimer;
        private readonly Dictionary<VolumeControlSubject, Dictionary<VolumeControlAction, HotkeyLib.KeyEventHandler?>> _actions;
        #endregion Members

        #region Events
        /// <summary>
        /// Triggered when the selected process changes.
        /// </summary>
        public event TargetEventHandler? SelectedProcessChanged;
        private void NotifySelectedProcessChanged(TargetEventArgs e)
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
                FLog.Log.WriteInfo($"Target Selection {(_selected_lock ? "Locked" : "Unlocked")}.");
            }
        }
        /// <summary>
        /// In milliseconds.
        /// </summary>
        public double HotkeyReloadInterval
        {
            get => _reloadTimer.Interval;
            set => _reloadTimer.Interval = value;
        }
        public bool ReloadOnHotkey { get; set; }
        #endregion Properties

        #region Methods
        public IAudioProcess? GetSelectedProcess() => _selected;
        public void SetSelectedProcess(string name, bool userOrigin = true)
        {
            if (!_selected_lock && ProcessList.Count > 0)
            {
                var it = ProcessList.FirstOrDefault(p => p != null && p.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase), null);
                if (it != null)
                    _selected = it;
                else _selected = new VirtualAudioProcess(name);
                NotifySelectedProcessChanged(new() { UserOrigin = userOrigin });
                FLog.Log.WriteDebug($"Target process name changed to '{_selected?.ProcessName}'");
            }
        }
        public void TrySetSelectedProcess(string? name, bool userOrigin = true)
        {
            if (name != null) SetSelectedProcess(name, userOrigin);
        }

        public int GetIndexWithProcessNameOrPID(object? process_identifier)
        {
            if (process_identifier is int pid)
                return GetIndexWithPID(pid);
            else if (process_identifier is string name)
            {
                name = name.Trim();
                if (name.All(char.IsDigit))
                {
                    int i = GetIndexWithPID(int.Parse(name));
                    if (i != -1) return i;
                }
                return GetIndexWithProcessName(name);
            }
            else return -1;
        }

        public int GetIndexWithProcessName(string name)
        {
            for (int i = 0, count = ProcessList.Count; i < count; ++i)
                if (ProcessList[i].ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }
        public int GetIndexWithPID(int pid)
        {
            for (int i = 0, count = ProcessList.Count; i < count; ++i)
                if (ProcessList[i].PID.Equals(pid))
                    return i;
            return -1;
        }
        public int GetSelectedIndex()
        {
            if (_selected == null)
                return -1;
            return GetIndexWithPID(_selected.PID);
        }
        /// <summary>
        /// Select the process after the current selection.
        /// If no process is selected, the first process is used instead.
        /// </summary>
        public void SelectNextProcess()
        {
            if (ReloadOnHotkey)
                TryReload();
            if (!_selected_lock && ProcessList.Count > 0)
            {
                int i = GetSelectedIndex();

                if (i == -1)
                    _selected = ProcessList.First();
                else
                    _selected = ProcessList[++i % ProcessList.Count];

                NotifySelectedProcessChanged(new() { UserOrigin = false });
                FLog.Log.WriteDebug($"Target process name changed to '{_selected.ProcessName}'");
            }
        }
        /// <summary>
        /// Select the process before the current selection.
        /// If no process is selected, the last process is used instead.
        /// </summary>
        public void SelectPrevProcess()
        {
            if (ReloadOnHotkey)
                TryReload();
            if (!_selected_lock && ProcessList.Count > 0)
            {
                int i = GetSelectedIndex();

                if (i == -1)
                    _selected = ProcessList.Last();
                else
                {
                    if (--i < 0)
                        i = ProcessList.Count - 1;
                    _selected = ProcessList[i];
                }

                NotifySelectedProcessChanged(new() { UserOrigin = false });
                FLog.Log.WriteDebug($"Target process name changed to '{_selected.ProcessName}'");
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

        private void TryReload()
        {
            if (_allowReload)
            {
                _allowReload = false;
                ReloadProcessList();
                _reloadTimer.Start();
            }
        }

        private void ReloadTimerElapsed(object sender, EventArgs e)
        {
            _reloadTimer.Stop();
            _allowReload = true;
        }

        public void SetHandler(VolumeControlSubject subject, VolumeControlAction action, HotkeyLib.KeyEventHandler handler)
            => _actions[subject][action] = handler;
        public HotkeyLib.KeyEventHandler? GetHandler(VolumeControlSubject subject, VolumeControlAction action)
            => _actions[subject][action];
        #endregion Methods
    }
}
