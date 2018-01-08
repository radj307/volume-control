using System;
using System.Collections.Generic;

namespace Toastify.Core
{
    public class ApplicationStartupException : ApplicationException
    {
        public ApplicationStartupException() : this(true)
        {
        }

        public ApplicationStartupException(bool spotifyStatus) : base(CreateMessage(spotifyStatus))
        {
        }

        public ApplicationStartupException(string message) : this(message, true)
        {
        }

        public ApplicationStartupException(string message, bool spotifyStatus) : base($"{CreateMessage(spotifyStatus)}\n\nAdditional details\n{message}")
        {
        }

        private static string CreateMessage(bool spotifyStatus = true)
        {
            List<string> messages = new List<string>();

            if (spotifyStatus)
            {
                bool spotifyInstanceCreated = Spotify.Instance != null;
                if (spotifyInstanceCreated)
                {
                    bool spotifyIsRunning = Spotify.Instance.IsRunning;
                    messages.Add($"Spotify is running: {spotifyIsRunning}");

                    var status = Spotify.Instance.Status;
                    messages.Add(status != null
                        ? $"Status: {status.Online}, {status.Running}, {status.Version}, {status.ClientVersion}, {status.Track != null}"
                        : "Status: null");
                }
                else
                    messages.Add("Spotify instance not created!");
            }

            return string.Join(Environment.NewLine, messages);
        }
    }
}