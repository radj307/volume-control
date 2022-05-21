﻿namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents read-only information about a process running on the local system.
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Gets the process name of this process.
        /// </summary>
        string ProcessName { get; }
        /// <summary>
        /// Gets the process ID number of this process.
        /// </summary>
        long PID { get; }
        /// <summary>
        /// Gets the process identifier of this process.
        /// </summary>
        /// <remarks>Uses this format:<br/><b>&lt;PID&gt;:&lt;ProcessName&gt;</b></remarks>
        string ProcessIdentifier { get; }
    }
}