using CommandLine;
using System.Diagnostics;
using VolumeControl.Log;
using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;

namespace VolumeControl.UpdateUtility;

internal static class Program
{
    private static LogWriter Log => FLog.Log;
    private static Mutex appMutex = null!;
    private static Options Args;

    /// <summary>
    /// Options object
    /// </summary>
    private class Options
    {
        [Option('p', "path", Required = true)]
        public string ExecutablePath { get; set; }

        [Option('u', "url", Required = true)]
        public string UpdateURL { get; set; }

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
        FLog.CustomInitialize(new ConsoleEndpoint(), EventType.ALL);
        try
        {
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

            ParserResult<Options>? argParser = Parser.Default.ParseArguments<Options>(args);
            Args = argParser.Value;

            appMutex = new(true, Args.MutexIdentifier, out bool acquiredMutexWithoutDelay);

            if (!acquiredMutexWithoutDelay) // hide the window until the mutex unlocks
                User32.ShowWindow(hWnd, User32.ECmdShow.SW_HIDE);

            if (appMutex.WaitOne(Args.Timeout))
            {
                if (!acquiredMutexWithoutDelay) // show the window now that we own the mutex
                    User32.ShowWindow(hWnd, User32.ECmdShow.SW_SHOW);

                Console.WriteLine($"Successfully acquired mutex lock, beginning update.");
                try
                {
                    argParser.WithParsed(Run).WithNotParsed(HandleParseError);
                }
                catch (Exception ex)
                {
                    Log.Fatal($"The update utility crashed because of an unhandled exception!", ex);
                }
            }
            else
            {
                Console.WriteLine("Failed to acquire mutex lock! Volume control didn't exit correctly.");
            }

            appMutex.ReleaseMutex();
            appMutex.Dispose();

            if (Args.RestartVolumeControl)
            {
                ProcessStartInfo psi = new(Args.ExecutablePath, "--autoupdated")
                {
                    UseShellExecute = true,
                };
                Log.Info("Restarting Volume Control...");
                Process.Start(psi);
            }
            else
            {
                Log.Info("Skipping restarting Volume Control.");
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex);
        }
    }
    private static void Run(Options args)
    {
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
        foreach (Error? error in errors)
        {
            Log.Error(error.ToString());
        }
    }
}
