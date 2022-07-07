namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// A log endpoint that implements <see cref="IEndpoint"/> and uses a <see cref="MemoryStream"/> as an endpoint.
    /// </summary>
    public class MemoryEndpoint : IEndpoint, IDisposable
    {
        #region Constructor
        /// <inheritdoc cref="MemoryEndpoint"/>
        /// <param name="enabled">When true, the endpoint starts enabled.</param>
        /// <param name="kilobytes">The size of the endpoint's buffer in kilobytes.</param>
        public MemoryEndpoint(bool enabled = true, int kilobytes = 10)
        {
            _stream = new(new byte[1024 * kilobytes], true);
            this.Enabled = enabled;
        }
        #endregion Constructor

        #region Fields
        private MemoryStream _stream;
        #endregion Fields

        #region Properties
        /// <inheritdoc/>
        public bool Enabled { get; set; }
        #endregion Properties

        #region Methods
        /// <inheritdoc/>
        public TextReader? GetReader() => this.Enabled ? new StreamReader(_stream) : null;
        /// <inheritdoc/>
        public TextWriter? GetWriter() => this.Enabled ? new StreamWriter(_stream) : null;
        /// <inheritdoc/>
        public int? ReadRaw()
        {
            if (!this.Enabled)
                return null;
            using TextReader? r = this.GetReader();
            int? ch = r?.Read();
            r?.Dispose();
            return ch;
        }
        /// <inheritdoc/>
        public string? ReadRawLine()
        {
            if (!this.Enabled)
                return null;
            using TextReader? r = this.GetReader();
            string? line = r?.ReadLine();
            r?.Dispose();
            return line;
        }
        /// <inheritdoc/>
        public void Reset() => _stream = new();
        /// <inheritdoc/>
        public void WriteRaw(string? str)
        {
            if (!this.Enabled || str == null)
                return;
            _stream.Write(new Span<byte>(str.ToCharArray().Cast<byte>().ToArray()));
        }
        /// <inheritdoc/>
        public void WriteRawLine(string? str = null)
        {
            if (!this.Enabled)
                return;
            this.WriteRaw(str == null ? "\n" : $"{str}\n");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ((IDisposable)_stream).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
