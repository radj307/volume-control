using Newtonsoft.Json;

namespace VolumeControl.Core.Structs
{
    /// <summary>
    /// Contains metadata for saving audio sessions to and from the config
    /// </summary>
    [JsonObject]
    public struct TargetInfo
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="TargetInfo"/> instance with default values.
        /// </summary>
        public TargetInfo()
        {
            PID = null;
            ProcessName = string.Empty;
            SessionInstanceIdentifier = string.Empty;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the process ID of this target.
        /// </summary>
        public uint? PID { get; set; }
        /// <summary>
        /// Gets or sets the process name of this target.
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// Gets the process identifier of this target in the form "PID:ProcessName"
        /// </summary>
        [JsonIgnore]
        public string ProcessIdentifier => $"{(PID.HasValue ? $"{PID}:" : "")}{ProcessName}";
        /// <summary>
        /// Gets or sets the session instance identifier of this target.
        /// </summary>
        public string SessionInstanceIdentifier { get; set; }
        #endregion Properties

        #region Statics
        /// <summary>
        /// Blank <see cref="TargetInfo"/> object.
        /// </summary>
        public static readonly TargetInfo Empty = new();
        #endregion Statics

        #region Operators
        /// <summary>
        /// Conversion operator from string
        /// </summary>
        public static explicit operator TargetInfo(string s) => new() { ProcessName = s };
        #endregion Operators
    }
}
