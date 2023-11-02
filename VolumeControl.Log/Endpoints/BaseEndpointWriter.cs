using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// <see langword="Abstract"/> base class for endpoint writers.
    /// </summary>
    public abstract class BaseEndpointWriter : IEndpointWriter, INotifyPropertyChanged
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="BaseEndpointWriter"/> instance.
        /// </summary>
        /// <param name="isWritingEnabled">The initial state </param>
        protected BaseEndpointWriter(bool isWritingEnabled = true)
        {
            _isWritingEnabled = isWritingEnabled;
        }
        #endregion Constructor

        #region Properties
        /// <inheritdoc/>
        public bool IsWritingEnabled
        {
            get => _isWritingEnabled;
            set
            {
                NotifyEnabledChanging(value);
                _isWritingEnabled = value;
                NotifyEnabledChanged(_isWritingEnabled);
            }
        }
        private bool _isWritingEnabled;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event EventHandler<bool>? EnabledChanging;
        private void NotifyEnabledChanging(bool incomingState) => EnabledChanging?.Invoke(this, incomingState);
        /// <inheritdoc/>
        public event EventHandler<bool>? EnabledChanged;
        private void NotifyEnabledChanged(bool newState) => EnabledChanged?.Invoke(this, newState);
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Triggers the PropertyChanged event for the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region (Abstract) Methods
        /// <inheritdoc/>
        public abstract TextWriter GetTextWriter();
        /// <inheritdoc/>
        public abstract void Reset();
        #endregion (Abstract) Methods

        #region Methods
        /// <inheritdoc/>
        public virtual void Write(string @string)
        {
            if (!IsWritingEnabled) return;

            using var writer = GetTextWriter();
            writer.Write(@string);
            writer.Flush();
        }
        /// <inheritdoc/>
        public virtual void WriteLine(string @string)
        {
            if (!IsWritingEnabled) return;

            using var writer = GetTextWriter();
            writer.WriteLine(@string);
            writer.Flush();
        }
        /// <summary>
        /// Writes a line break to the endpoint.
        /// </summary>
        public virtual void WriteLine() => WriteLine(string.Empty);
        #endregion Methods
    }
}