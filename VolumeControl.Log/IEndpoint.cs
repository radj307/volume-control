namespace VolumeControl.Log
{
    public interface IEndpoint
    {
        /// <summary>
        /// Return true when endpoint is ready to read/write.
        /// </summary>
        bool Ready { get; }
        /// <summary>
        /// Write to the filestream.
        /// It is highly recommended that you do not use this function, as it doesn't conform to formatting rules.
        /// </summary>
        /// <param name="str">A string to write.</param>
        void WriteRaw(string? str, FileMode mode = FileMode.Append);
        /// <summary>
        /// Write to the filestream, and append a newline.
        /// It is highly recommended that you do not use this function, as it doesn't conform to formatting rules.
        /// </summary>
        /// <param name="str">A string to write.</param>
        void WriteRawLine(string? str = null, FileMode mode = FileMode.Append);

        /// <summary>
        /// Read a character from the log endpoint.
        /// </summary>
        /// <returns>Integer (4-byte) representation of a single character.</returns>
        int? ReadRaw(FileMode mode);
        /// <summary>
        /// Read a line from the log endpoint.
        /// </summary>
        /// <returns>A string containing one line from the log endpoint.</returns>
        string? ReadRawLine(FileMode mode);
        /// <summary>
        /// Reset the contents of the log endpoint, leaving it empty.
        /// </summary>
        void Reset();
    }
}