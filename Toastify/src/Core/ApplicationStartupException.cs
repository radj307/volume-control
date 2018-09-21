using System;
using System.Collections.Generic;

namespace Toastify.Core
{
    public class ApplicationStartupException : ApplicationException
    {
        public ApplicationStartupException(string message) : base(message + Environment.NewLine + CreateMessage())
        {
        }

        #region Static Members

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

            List<string> messages = new List<string>
            {
                $"\tSpotify instance running status: {Spotify.Instance.IsRunning}"
            };

            return string.Join(Environment.NewLine, messages);
        }

        #endregion
    }
}