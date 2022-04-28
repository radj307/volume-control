using System.Reflection;
using VolumeControl.Core.Attributes;

namespace VolumeControl.CLI
{
    internal static class Program
    {
        private static bool _quiet = false;

        [STAThread]
        private static void Main(string[] argv)
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
                    return;
                }
                else if (argv.Any(a => a.Equals("--version")))
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    string? strver = assembly.GetCustomAttribute<ExtendedVersion>()?.Version;
                    Console.WriteLine(strver);
                    return;
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
