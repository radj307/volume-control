namespace VolumeControl.Log.Interfaces
{
    /// <summary>
    /// Represents an endpoint that text can be written to.
    /// </summary>
    public interface IEndpointWriter
    {
        #region Properties
        /// <summary>
        /// Gets or sets whether this endpoint can be written to.
        /// </summary>
        bool IsWritingEnabled { get; set; }
        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs when the endpoint is about to be enabled or disabled for any reason.
        /// </summary>
        /// <remarks>
        /// The boolean argument is the incoming state.
        /// </remarks>
        event EventHandler<bool>? EnabledChanging;
        /// <summary>
        /// Occurs when the endpoint is enabled or disabled for any reason.
        /// </summary>
        /// <remarks>
        /// The boolean argument is the new state.
        /// </remarks>
        event EventHandler<bool>? EnabledChanged;
        #endregion Events

        #region Methods
        /// <summary>
        /// Gets a <see cref="TextWriter"/> object for writing to this endpoint.
        /// </summary>
        /// <remarks>
        /// The caller is responsible for disposing of the stream.
        /// </remarks>
        /// <returns>A <see cref="TextWriter"/> instance for this endpoint when it is enabled; otherwise <see langword="null"/>.</returns>
        TextWriter GetTextWriter();
        /// <summary>
        /// Writes the specified <paramref name="string"/> to the endpoint.
        /// </summary>
        /// <param name="string">A string to write to the endpoint.</param>
        void Write(string @string);
        /// <summary>
        /// Writes the specified <paramref name="string"/> to the endpoint, followed by a line break.
        /// </summary>
        /// <param name="string">A string to write to the endpoint.</param>
        void WriteLine(string @string);
        /// <summary>
        /// Clears the endpoint's contents, leaving it blank.
        /// </summary>
        void Reset();
        #endregion Methods
    }
}