namespace VolumeControl.Log.Endpoints
{
    public interface IEndpoint
    {
        /// <summary>
        /// Return true when the endpoint is enabled.
        /// </summary>
        bool Enabled { get; internal set; }
        /// <summary>
        /// Retrieve a <see cref="StreamReader"/> object for reading from the endpoint.<br></br>
        /// Using this is only recommended for repeated read operations, such as in a loop; There is no benefit to using this for single read operations.
        /// </summary>
        /// <returns><list type="table">
        /// <item><term>null</term><description>The endpoint isn't enabled.</description></item>
        /// <item><term><see cref="StreamReader"/></term><description>A reader using this endpoint's output target.</description></item>
        /// </list></returns>
        StreamReader? GetReader();
        /// <summary>
        /// Retrieve a <see cref="StreamWriter"/> object for writing to the endpoint.<br></br>
        /// Using this is only recommended for repeated write operations, such as in a loop; There is no benefit to using this for single write operations.
        /// </summary>
        /// <returns><list type="table">
        /// <item><term>null</term><description>The endpoint isn't enabled.</description></item>
        /// <item><term><see cref="StreamWriter"/></term><description>A writer using this endpoint's output target.</description></item>
        /// </list></returns>
        StreamWriter? GetWriter();
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