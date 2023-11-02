namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// Allows using the <see cref="Console"/> as a logging endpoint.
    /// </summary>
    public class ConsoleEndpoint : BaseEndpointWriter
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ConsoleEndpoint"/> instance.
        /// </summary>
        public ConsoleEndpoint() : base(true) { }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Gets the <see cref="TextWriter"/> instance for the standard output console stream.
        /// </summary>
        /// <remarks>
        /// The caller should not dispose of the returned writer object.
        /// </remarks>
        /// <returns><see cref="TextWriter"/> instance.</returns>
        public static TextWriter GetSTDOUT() => Console.Out;
        /// <summary>
        /// Gets the <see cref="TextWriter"/> instance for the standard error console stream.
        /// </summary>
        /// <remarks>
        /// The caller should not dispose of the returned writer object.
        /// </remarks>
        /// <returns><see cref="TextWriter"/> instance.</returns>
        public static TextWriter GetSTDERR() => Console.Error;
        /// <inheritdoc cref="GetSTDOUT"/>
        public override TextWriter GetTextWriter() => Console.Out;
        /// <summary>
        /// Clears the console.
        /// </summary>
        public override void Reset() => Console.Clear();
        #endregion Methods
    }
}
