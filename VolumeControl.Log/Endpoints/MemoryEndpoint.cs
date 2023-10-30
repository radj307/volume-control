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
        public bool Enabled
        {
            get => _enabled;
            set
            {
                NotifyEnabledChanging(value);
                _enabled = value;
                NotifyEnabledChanged(_enabled);
            }
        }
        private bool _enabled;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event EventHandler<bool>? EnabledChanging;
        private void NotifyEnabledChanging(bool incomingState) => EnabledChanging?.Invoke(this, incomingState);
        /// <inheritdoc/>
        public event EventHandler<bool>? EnabledChanged;
        private void NotifyEnabledChanged(bool newState) => EnabledChanged?.Invoke(this, newState);
        #endregion Events

        #region Methods
        /// <inheritdoc/>
        public TextReader? GetReader() => this.Enabled ? new StreamReader(_stream, leaveOpen: true) : null;
        /// <inheritdoc/>
        public TextWriter? GetWriter() => this.Enabled ? new StreamWriter(_stream, leaveOpen: true) : null;
        /// <inheritdoc/>
        public int? ReadRaw()
        {
            if (!this.Enabled)
                return null;
            using TextReader? r = this.GetReader();
            int? ch = r?.Read();
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

        #region Method Overrides
        /// <summary>
        /// Returns the contents of the stream as a UTF8 string.
        /// </summary>
        /// <returns><see cref="string"/> with the entire contents of the memory stream.</returns>
        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(_stream.ToArray().TakeWhile(b => b != '\0').ToArray());
        }
        #endregion Method Overrides
    }
}
