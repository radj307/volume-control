using System;
using System.Collections.Generic;

namespace Toastify.Core
{
    public class ApplicationStartupException : ApplicationException
    {
        public ApplicationStartupException(string message) : base(message + Environment.NewLine + CreateMessage())
        {
        }

        private static string CreateMessage()
        {
            // Touching Spotify.Instance forces instance creation if none, but it fails if called too early. 
            // Therefore, a try is needed.
            try
            {
                // unused variable is needed to build the project
                var burner = Spotify.Instance;
            }
            catch (NullReferenceException)
            {
                return "\tSpotify instance status: not yet created.";
            }

            List<string> messages = new List<string>();
            messages.Add($"\tSpotify instance running status: {Spotify.Instance.IsRunning}");

            var status = Spotify.Instance.Status;
            if (status == null)
                messages.Add("\tSpotify instance status: null");
            else
            {
                messages.Add("\tSpotify instance status: ");
                messages.Add($"\t-- Version:\t{status.Version}");
                messages.Add($"\t-- Client Version:\t{status.ClientVersion}");
                messages.Add($"\t-- Playing:\t{status.Playing}");
                messages.Add($"\t-- Shuffle:\t{status.Shuffle}");
                messages.Add($"\t-- Repeat:\t{status.Repeat}");
                messages.Add($"\t-- Play Enabled:\t{status.PlayEnabled}");
                messages.Add($"\t-- Prev Enabled:\t{status.PrevEnabled}");
                messages.Add($"\t-- Next Enabled:\t{status.NextEnabled}");
                messages.Add($"\t-- has Track:\t{status.Track != null}");
                messages.Add($"\t-- Server Time:\t{status.ServerTime}");
                messages.Add($"\t-- Volume:\t{status.Volume}");
                messages.Add($"\t-- Online:\t{status.Online}");
                messages.Add($"\t-- has OpenGraphState: {status.OpenGraphState != null}");
                messages.Add($"\t-- Running:\t{status.Running}");
            }

            return string.Join(Environment.NewLine, messages);
        }
    }
}