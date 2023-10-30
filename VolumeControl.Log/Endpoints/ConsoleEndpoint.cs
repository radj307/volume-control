namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// Allows using the <see cref="Console"/> as a logging endpoint.
    /// </summary>
    public class ConsoleEndpoint : IEndpoint
    {
        #region Constructor
        /// <inheritdoc cref="ConsoleEndpoint"/>
        public ConsoleEndpoint() => this.Enabled = true;
        #endregion Constructor

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
        public TextReader? GetReader() => Console.In;
        /// <inheritdoc/>
        public TextWriter? GetWriter() => Console.Out;
        /// <inheritdoc/>
        public int? ReadRaw() => Console.Read();
        /// <inheritdoc/>
        public string? ReadRawLine() => Console.ReadLine();
        /// <inheritdoc/>
        public void Reset() => Console.Clear();
        /// <inheritdoc/>
        public void WriteRaw(string? str) => Console.Write(str);
        /// <inheritdoc/>
        public void WriteRawLine(string? str = null) => Console.WriteLine(str);
        #endregion Methods
    }
}
