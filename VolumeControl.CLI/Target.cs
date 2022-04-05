using AudioAPI;
using System.Diagnostics;

namespace VolumeControl.CLI
{
    internal class Target
    {
        public Target(string? target = null) { Set(target); }

        private string? _str;
        private Core.Audio.AudioProcess? _proc;

        public void Set(string? target)
        {
            _str = target;
            if (_str != null)
            {
                if (_str.All(c => char.IsDigit(c)))
                    _proc = new(Process.GetProcessById(Convert.ToInt32(_str)));
                else
                {
                    var proc = Process.GetProcessesByName(_str).FirstOrDefault(p => true, null);
                    if (proc != null)
                        _proc = new(proc);
                    else
                        _proc = null;
                }
            }
            else _proc = null;
        }

        public bool IsValid => _proc != null;
        public string ProcessName
        {
            get
            {
                if (IsValid) return _proc!.ProcessName;
                else throw new InvalidOperationException($"'{_str}' doesn't specify a valid (running) process name or ID!");
            }
        }
        public int PID
        {
            get
            {
                if (IsValid) return _proc!.PID;
                else throw new InvalidOperationException($"'{_str}' doesn't specify a valid (running) process name or ID!");
            }
        }

        public bool Muted
        {
            get => VolumeHelper.IsMuted(PID);
            set => VolumeHelper.SetMute(PID, value);
        }
        public int Volume
        {
            get => Convert.ToInt32(VolumeHelper.GetVolume(PID));
            set => VolumeHelper.SetVolume(PID, Convert.ToDecimal(value));
        }
        public void SetVolume(string? volumestr)
        {
            if (!IsValid)
                throw new InvalidOperationException($"'{_str}' doesn't specify a valid (running) process name or ID!");
            else if (volumestr == null)
                throw new ArgumentNullException(nameof(volumestr), $"Cannot set volume of '{ProcessName}' to null!");
            else if (volumestr[1..].All(char.IsDigit))
            {
                if (char.IsDigit(volumestr[0]))
                    VolumeHelper.SetVolume(PID, Convert.ToDecimal(volumestr));
                else
                {
                    char specifier = volumestr[0];
                    volumestr = volumestr[1..];
                    switch (specifier)
                    {
                    case '+':
                        VolumeHelper.IncrementVolume(PID, Convert.ToDecimal(volumestr));
                        break;
                    case '-':
                        VolumeHelper.DecrementVolume(PID, Convert.ToDecimal(volumestr));
                        break;
                    default:
                        throw new ArgumentException($"Invalid volume prefix character '{specifier}'!");
                    }
                }
            }
            else throw new ArgumentException($"Cannot set volume of '{ProcessName}' to invalid integral '{volumestr}'!");
        }
        public void SetMuted(string? mutedstr)
        {
            bool? mute = null;
            if (!IsValid)
                throw new InvalidOperationException($"'{_str}' doesn't specify a valid (running) process name or ID!");
            else if (mutedstr == null)
                throw new ArgumentNullException(nameof(mutedstr), $"Cannot set mute state of '{ProcessName}' to null!");
            else if (mutedstr.All(char.IsDigit))
                mute = Convert.ToBoolean(Convert.ToInt32(mutedstr));
            else if (mutedstr.Equals("true", StringComparison.OrdinalIgnoreCase))
                mute = true;
            else if (mutedstr.Equals("false", StringComparison.OrdinalIgnoreCase))
                mute = false;
            else throw new ArgumentException($"Cannot set mute state of '{ProcessName}' to invalid boolean '{mutedstr}'!");
            if (mute == null)
                throw new Exception($"Failed to parse a valid boolean value from '{mutedstr}'!");
            Muted = mute.Value;
        }
        public override string? ToString()
        {
            if (!IsValid)
                return null;
            return $"[{PID}] '{ProcessName}'";
        }
    }
}
