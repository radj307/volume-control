using Newtonsoft.Json;

namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Contains metadata for saving audio sessions to and from the config
    /// </summary>
    [JsonObject]
    public struct TargetInfo
    {
        /// <summary>
        /// Process identifier in the form "PID:PNAME"
        /// </summary>
        public string ProcessIdentifier { get; set; }
        /// <summary>
        /// Session instance identifier
        /// </summary>
        public string SessionInstanceIdentifier { get; set; }
        /// <summary>
        /// Blank <see cref="TargetInfo"/> object.
        /// </summary>
        public static readonly TargetInfo Empty = new() { ProcessIdentifier = string.Empty, SessionInstanceIdentifier = string.Empty };
        /// <summary>
        /// Conversion operator from string
        /// </summary>
        public static explicit operator TargetInfo(string s) => new() { ProcessIdentifier = s, SessionInstanceIdentifier = string.Empty };

        /// <summary>
        /// Gets the process ID by parsing <see cref="ProcessIdentifier"/>
        /// </summary>
        /// <returns>The PID number as an <see cref="int"/> if one was found; otherwise <see langword="null"/>.</returns>
        public int? GetProcessID()
        {
            var idx = ProcessIdentifier.IndexOf(':');
            if (idx == -1)
            {
                if (ProcessIdentifier.All(char.IsDigit))
                    return Convert.ToInt32(ProcessIdentifier);
                else
                    return null;
            }

            return Convert.ToInt32(ProcessIdentifier[..idx]);
        }
        /// <summary>
        /// Gets the process name by parsing <see cref="ProcessIdentifier"/>
        /// </summary>
        /// <returns>The process name as a <see cref="string"/> if one was found; otherwise <see cref="string.Empty"/>.</returns>
        public string GetProcessName()
        {
            var idx = ProcessIdentifier.IndexOf(':');
            if (idx == -1)
            {
                if (ProcessIdentifier.All(char.IsDigit))
                    return string.Empty;
                else
                    return ProcessIdentifier;
            }

            return ProcessIdentifier[(idx + 1)..];
        }
    }
}
