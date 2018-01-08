using System;
using System.Collections.Generic;

namespace Toastify.Core
{
    public class ApplicationStartupException : ApplicationException
    {
        public ApplicationStartupException() : base(CreateMessage())
        {
        }

        public ApplicationStartupException(string message) : base($"{CreateMessage()}\n\nAdditional details\n{message}")
        {
        }

        private static string CreateMessage()
        {
            List<string> messages = new List<string>();

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

            return string.Join(Environment.NewLine, messages);
        }
    }
}