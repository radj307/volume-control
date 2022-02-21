using AudioAPI;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VolumeControl.CLI;

internal enum Action : byte
{
    SET,
    GET,
}
internal enum Target : byte
{
    VOLUME,
    MUTED,
}

public class Program
{
    private static bool IsDigit(char c) => c >= '0' && c <= '9';
    public static int Main(string[] args)
    {
        try
        {
            // exit early if no arguments
            if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine("VolumeControlCLI");
                Console.WriteLine("  CLI application for controlling the volume of specific Windows applications.");
                Console.WriteLine();
                Console.WriteLine("USAGE:");
                Console.WriteLine("  VolumeControlCLI <-p [ProcessName]> [-v [VolumeLevel]] [--mute|--unmute] [--set|--get]");
                Console.WriteLine();
                Console.WriteLine("OPTIONS:");
                Console.WriteLine("  -h  --help                           Show this help display, then exit.");
                Console.WriteLine("  -V  --version                        Show the current version number, then exit.");
                Console.WriteLine("  -p <PROCESS>  --process <PROCESS>    Select the target process' name or ID.");
                Console.WriteLine("  -v <VOLUME>   --volume <VOLUME>      Specify a volume level to set the process to.");
                Console.WriteLine("  --mute                               Mute the target application.");
                Console.WriteLine("  --unmute                             Unmute the target application.");
                Console.WriteLine("  -s  --set                            Specify that the target application's volume should be set.");
                Console.WriteLine("                                       This option is implied when [-v|--volume] is specified.");
                Console.WriteLine("  -g  --get                            Specify that the target application's volume should be queried.");
                Console.WriteLine("                                       This option is implied when [-v|--volume] is not specified.");
                if (args.Length == 0)
                    throw new ArgumentException("No valid arguments specified!");
                else return 0;
            }
            else if (args.Contains("-V") || args.Contains("--version"))
            {
                Version currentVersion = typeof(Program).Assembly.GetName().Version!;
                if (Convert.ToBoolean(typeof(Program).Assembly.GetCustomAttribute<IsPreReleaseAttribute>()?.IsPreRelease)) // pre release
                {
                    Console.WriteLine($"VolumeControlCLI  v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}-pre{currentVersion.Revision}");
                }
                else // not a pre release
                {
                    Console.WriteLine($"VolumeControlCLI  v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}{(currentVersion.Revision >= 1 ? $"-{currentVersion.Revision}" : "")}");
                }
                return 0;
            }

            string? processName = null;
            int? processId = null;
            decimal? volumeLevel = null;
            bool? setMute = null;
            Action action = Action.SET;
            List<Target> targets = new();

            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                int setterPos = arg.IndexOf('=');

                // [-p|--process]:
                if (arg.StartsWith("-p", StringComparison.Ordinal) || arg.StartsWith("--process", StringComparison.Ordinal))
                {
                    if (setterPos != -1)
                        processName = arg[(setterPos + 1)..];
                    else if (i + 1 < args.Length)
                    {
                        string nextArg = args[++i];
                        if (!nextArg.StartsWith('-'))
                        {
                            if (nextArg.All(ch => IsDigit(ch)))
                                processId = Convert.ToInt32(nextArg);
                            else
                                processName = nextArg;
                        }
                        else --i;
                    }
                    if (processName == null && processId == null)
                        throw new ArgumentException("Failed to find a process name or ID after the specifier!");
                }
                // [-v|--volume]:
                else if (arg.StartsWith("-v", StringComparison.Ordinal) || arg.StartsWith("--volume", StringComparison.Ordinal))
                {
                    targets.Add(Target.VOLUME);
                    if (setterPos != -1)
                        volumeLevel = Convert.ToDecimal(arg[(setterPos + 1)..]);
                    else if (i + 1 < args.Length)
                    {
                        string nextArg = args[++i];
                        if (!nextArg.StartsWith('-'))
                            volumeLevel = Convert.ToDecimal(nextArg);
                        else --i;
                    }
                }
                // [--mute]:
                else if (arg.Equals("--mute"))
                {
                    targets.Add(Target.MUTED);
                    setMute = true;
                }
                // [--unmute]:
                else if (arg.Equals("--unmute"))
                {
                    targets.Add(Target.MUTED);
                    setMute = false;
                }
                // [-s|--set]:
                else if (arg.Equals("-s") || arg.Equals("--set"))
                    action = Action.SET;
                // [-g|--get]:
                else if (arg.Equals("-g") || arg.Equals("--get"))
                    action = Action.GET;
            }

            if (processName == null && processId == null)
                throw new ArgumentException("No process name or ID specified!");

            if (targets.Count == 0)
                throw new ArgumentException("No targets specified!");

            if (action == Action.SET)
            {
                if (targets.Contains(Target.VOLUME))
                {
                    if (volumeLevel == null)
                        throw new Exception("Cannot set volume level; No value provided!");
                    if (processName != null)
                        VolumeHelper.SetVolume(processName, Convert.ToDecimal(volumeLevel));
                    else if (processId != null)
                        VolumeHelper.SetVolume(Convert.ToInt32(processId), Convert.ToDecimal(volumeLevel));
                    Console.WriteLine($"{processName} volume set to {volumeLevel}.");
                }
                if (targets.Contains(Target.MUTED))
                {
                    if (setMute == null)
                        throw new Exception("Cannot set mute state; No value provided!");
                    bool mute = Convert.ToBoolean(setMute);
                    if (processName != null)
                        VolumeHelper.SetMute(processName, mute);
                    else if (processId != null)
                        VolumeHelper.SetMute(Convert.ToInt32(processId), mute);
                    Console.WriteLine($"{processName} {(mute ? "" : "un")}muted.");
                }
            }
            else if (action == Action.GET)
            {
                if (targets.Contains(Target.VOLUME))
                {
                    if (processName != null)
                        Console.WriteLine($"{processName} volume:  {VolumeHelper.GetVolume(processName)}");
                    else if (processId != null)
                        Console.WriteLine($"Process {processId} volume:  {VolumeHelper.GetVolume(Convert.ToInt32(processId))}");
                }
                if (targets.Contains(Target.MUTED))
                {
                    if (processName != null)
                        Console.WriteLine($"{processName} muted:  {VolumeHelper.IsMuted(processName)}");
                    else if (processId != null)
                        Console.WriteLine($"Process {processId} muted:  {VolumeHelper.IsMuted(Convert.ToInt32(processId))}");
                }
            }
            else throw new Exception("No valid action specified!");

            return 0;
        }
        catch (COMException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{ex.Message}!\nThe specified process doesn't have an active audio session!");
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (ArgumentException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid Arguments!\n{ex.Message}\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An exception occurred!\n{ex.Message}\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        return 1;
    }
}
