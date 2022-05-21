using CommandLine;
using System;
using System.Diagnostics;
using VolumeControl.Log;
using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;

namespace VolumeControl.UpdateUtility;

static class Program
{
    private static LogWriter Log => FLog.Log;
    private static Mutex appMutex = null!;
    private class Options
    {
        [Option('p', "path", Required = true)]
        public string ExecutablePath { get; set; }
        [Option('u', "url", Required = true)]
        public string UpdateURL { get; set; }
        public EventType LogFilter { get; set; } = EventType.ALL_EXCEPT_DEBUG;
        [Option("deletebackup", Default = true)]
        public bool DeleteBackup { get; set; }
        [Option('t', "timeout", Default = -1)]
        public int Timeout { get; set; }
        [Option('m', "mutex", Default = "VolumeControlSingleInstance")]
        public string MutexIdentifier { get; set; }
        [Option('d', "dry-run", Default = false)]
        public bool DryRun { get; set; }
        [Option('r', "restart-main", Default = true)]
        public bool RestartVolumeControl { get; set; }
    }

    [STAThread]
    public static void Main(string[] args)
    {
        IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

        var argParser = Parser.Default.ParseArguments<Options>(args);
        appMutex = new Mutex(true, argParser.Value.MutexIdentifier, out bool acquiredMutexWithoutDelay);

        if (!acquiredMutexWithoutDelay)
            User32.ShowWindow(hWnd, User32.ECmdShow.SW_HIDE);

        if (appMutex.WaitOne(argParser.Value.Timeout))
        {
            if (!acquiredMutexWithoutDelay)
                User32.ShowWindow(hWnd, User32.ECmdShow.SW_SHOW);

            Console.WriteLine($"Successfully acquired mutex lock, beginning update.");
            try
            {
                argParser.WithParsed(Run).WithNotParsed(HandleParseError);
            }
            catch (Exception ex)
            {
                PrintError($"The update utility crashed because of an unhandled exception!", ex.Message, ex.InnerException, ex.StackTrace);
            }
        }
        else Console.WriteLine("Failed to acquire mutex lock! Volume control didn't exit correctly.");

        appMutex.ReleaseMutex();
        appMutex.Dispose();

        if (argParser.Value.RestartVolumeControl)
        {
            ProcessStartInfo psi = new(argParser.Value.ExecutablePath, "--autoupdated")
            {
                UseShellExecute = true,
            };
            Log.Info("Restarting Volume Control...");
            Process.Start(psi);
        }
        else Log.Info("Skipping restarting Volume Control.");

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
    private static void Run(Options args)
    {
        FLog.CustomInitialize(new ConsoleEndpoint(), args.LogFilter);

        Log.Info($"Target: {args.ExecutablePath}", $"URL:  {args.UpdateURL}");

        bool alreadyExists = File.Exists(args.ExecutablePath);

        string filename = Path.GetFileName(args.ExecutablePath)!;
        string location = Path.GetDirectoryName(args.ExecutablePath)!;
        string? backupPath = alreadyExists ? Path.Combine(location, $"{filename}.backup") : null;

        if (alreadyExists)
        { // rename the current file
            if (File.Exists(backupPath))
                File.Delete(backupPath);
            File.Move(args.ExecutablePath, backupPath!);
            Log.Info($"Created backup: '{args.ExecutablePath}' => '{backupPath}'");
        }

        // make an HTTP GET request for the executable file
        using HttpClient client = new();
        // Set the request header to request the raw executable file
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new("application/octet-stream"));
        client.DefaultRequestHeaders.Add("User-Agent", "curl/7.64.1");

        if (HttpDownloadFileAsync(client, args.UpdateURL, args.ExecutablePath).Wait(-1))
        { // success
            Log.Info($"Successfully downloaded '{args.ExecutablePath}'");
            if (args.DeleteBackup && backupPath != null && File.Exists(backupPath))
            { // if we reached this point successfully, delete the backup
                try
                {
                    File.Delete(backupPath);
                    Log.Info($"Deleted backup '{backupPath}'");
                }
                catch (Exception ex)
                {
                    Log.Error("An exception was thrown while deleting the backup file!", ex);
                }
            }

            Log.Info("Successfully updated Volume Control.");
        }
        else
        { // timeout
            Log.Error($"Timed out while waiting for download to complete!");
            if (alreadyExists)
            {
                File.Move(backupPath!, args.ExecutablePath);
                Log.Info($"Restored backup: '{backupPath}' => '{args.ExecutablePath}'");
            }
        }

        client.Dispose();
    }
    /// <summary>Downloads a file from the specified URL directly to disk, rather than caching the entire thing in memory first.</summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for the request.</param>
    /// <param name="url">The target URL</param>
    /// <param name="fileToWriteTo">The output filepath.</param>
    private static async Task HttpDownloadFileAsync(HttpClient httpClient, string url, string fileToWriteTo)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
        using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
        await streamToReadFrom.CopyToAsync(streamToWriteTo);
    }
    private static void HandleParseError(IEnumerable<Error> errors)
    {
        foreach (var error in errors)
        {
            PrintError(error.ToString());
        }
    }
    private static void PrintError(params object?[] lines)
    {
        if (lines.Length == 0 || (lines[0] == null && lines.Length == 1))
            return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR]");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("  ");
        Console.WriteLine(lines[0]);

        const int margin = 9;
        string padding = new(' ', margin);

        if (lines.Length > 1)
        {
            foreach (var line in lines[1..])
            {
                if (line == null)
                    continue;
                Console.WriteLine($"{padding}{line}");
            }
        }
    }
}
