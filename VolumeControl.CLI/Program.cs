using AudioAPI;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using VolumeControl.Core;

namespace VolumeControl.CLI
{
    internal class Target
    {
        public Target(string? target = null) { Set(target); }

        private string? _str;
        private AudioProcess? _proc;

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

    internal static class Program
    {
        private static bool _quiet = false;

        [STAThread]
        static void Main(string[] argv)
        {
            try
            {
                #region Help
                if (argv.Length == 0 || argv.Any(a => a.Equals("-h") || a.Equals("--help")))
                {
                    string myName = Assembly.GetCallingAssembly().GetName().Name ?? "VolumeControl.CLI";
                    Console.WriteLine($"{myName}");
                    Console.WriteLine("  ");
                    Console.WriteLine();
                    Console.WriteLine("Usage:");
                    Console.WriteLine($"  {myName} <TARGET> [[OPTIONS]...]");
                    Console.WriteLine();
                    Console.WriteLine("  Both the Process Name and/or PID number may be used to specify the target, which is always the first argument.");
                    Console.WriteLine("  ");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                }
                else if (argv.Any(a => a.Equals("--version")))
                {
                    string strver = null!;
                    Version currentVersion = typeof(Form).Assembly.GetName().Version!;
                    if (Convert.ToBoolean(typeof(Form).Assembly.GetCustomAttribute<IsPreReleaseAttribute>()?.IsPreRelease))
                        strver = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}-pre{currentVersion.Revision}";
                    else
                        strver = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}{(currentVersion.Revision >= 1 ? $"-{currentVersion.Revision}" : "")}";
                    Console.WriteLine(strver);
                }
                #endregion Help

                Target target = new(argv[0]);

                for (int i = 1; i < argv.Length; ++i)
                {
                    string argument = argv[i];
                    if (argument.StartsWith('-'))
                    {
                        string arg = argument[1..];
                        if (arg.StartsWith('-')) // option
                        {
                            arg = arg[1..];

                            if (arg.Equals("quiet"))
                            {
                                _quiet = true;
                                continue;
                            }

                            string specifier = arg[..3];
                            if (arg[3] != '-') throw new ArgumentException($"Invalid option syntax '{argument}'!");
                            arg = arg[4..];

                            string? value = null;
                            if (arg.Contains('='))
                            {
                                int index = arg.IndexOf('=');
                                value = arg[(index + 1)..];
                                arg = arg[..index];
                            }
                            if (value == null && i + 1 < argv.Length)
                            {
                                value = argv[++i];
                            }

                            if (specifier.Equals("set"))
                            { // setter
                                if (value == null)
                                    throw new ArgumentException($"No value specified for setter argument '{argument}'!");
                                if (arg.Equals("volume"))
                                {
                                    target.SetVolume(value);
                                    if (!_quiet) Console.WriteLine($"{target} volume = {target.Volume}");
                                }
                                else if (arg.Equals("muted"))
                                {
                                    target.SetMuted(value);
                                    if (!_quiet) Console.WriteLine($"{target} muted = {target.Muted}");
                                }
                            }
                            else if (specifier.Equals("get"))
                            { // getter
                                if (arg.Equals("volume"))
                                {
                                    Console.WriteLine($"{(_quiet ? "" : $"{target} volume = ")}{target.Volume}");
                                }
                                else if (arg.Equals("muted"))
                                {
                                    Console.WriteLine($"{(_quiet ? "" : $"{target} muted = ")}{target.Muted}");
                                }
                            }
                            else throw new ArgumentException($"Invalid option syntax '{argument}'!");
                        }
                        else // flag
                        {
                            string? value = null;
                            if (arg.Contains('='))
                            {
                                int index = arg.IndexOf('=');
                                value = arg[(index + 1)..];
                                arg = arg[..index];
                            }
                            if (value == null && i + 1 < argv.Length)
                            {
                                value = argv[++i];
                            }

                            bool? setter = null;
                            foreach (char flag in arg) // parse flagchain
                            {
                                switch (flag)
                                {
                                case 'q':
                                    _quiet = true;
                                    break;
                                case 's':
                                    setter = true;
                                    break;
                                case 'g':
                                    setter = false;
                                    break;
                                case 'V':
                                    if (setter == null)
                                        throw new ArgumentException($"Invalid syntax in flag '{argument}', flag 'V' must be preceeded by 's' or 'g' in the same chain!");
                                    else if (setter.Value)
                                    { // set volume
                                        target.SetVolume(value);
                                        if (!_quiet) Console.WriteLine($"{target} volume = {target.Volume}");
                                    }
                                    else
                                    { // get volume
                                        Console.WriteLine($"{(_quiet ? "" : $"{target} volume = ")}{target.Volume}");
                                    }
                                    break;
                                case 'M':
                                    if (setter == null)
                                        throw new ArgumentException($"Invalid syntax in flag '{argument}', flag 'M' must be preceeded by 's' or 'g' in the same chain!");
                                    else if (setter.Value)
                                    { // set muted
                                        target.SetMuted(value);
                                        if (!_quiet) Console.WriteLine($"{target} muted = {target.Muted}");
                                    }
                                    else
                                    { // get muted
                                        Console.WriteLine($"{(_quiet ? "" : $"{target} muted = ")}{target.Muted}");
                                    }
                                    break;
                                default:
                                    throw new ArgumentException($"Unrecognized flag '{flag}' in '{argument}'!");
                                }
                            }
                        }
                    }
                    else throw new ArgumentException($"Uncaptured argument '{argument}' is invalid! (Try '-h' or '--help')"); // parameter
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR]  {e.Message}");
                Console.WriteLine($"         {e.Data}");
            }
#           if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.Read();
#           endif
        }
    }
}
