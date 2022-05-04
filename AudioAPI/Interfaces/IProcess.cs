using System.Diagnostics;

namespace AudioAPI.Interfaces
{
    /// <summary>
    /// Represents a <see cref="Process"/> that may be running on the local system.
    /// </summary>
    public interface IProcess : IEquatable<IProcess>
    {
        /// <summary>
        /// Gets whether or not this is a real <see cref="Process"/>.
        /// </summary>
        bool Virtual { get; }
        /// <summary>
        /// Gets the process name of this process, or an empty string if this process is virtual.
        /// </summary>
        string ProcessName { get; }
        /// <summary>
        /// Gets the process ID of this process, or -1 if this process is virtual.
        /// </summary>
        int PID { get; }
    }
}
