using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// Log endpoint that allows writing logs directly to a file on disk.
    /// </summary>
    public class FileEndpoint : BaseEndpointWriter, IEndpointWriter
    {
        #region Constructors
        /// <inheritdoc cref="FileEndpoint"/>
        /// <param name="path">The location of the log file.</param>
        /// <param name="enabled">Whether the endpoint is already enabled when constructed or not.</param>
        public FileEndpoint(string path, bool enabled) : base(enabled)
        {
            _path = path;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// The location of the log file.
        /// </summary>
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                NotifyPropertyChanged();
            }
        }
        private string _path;
        /// <summary>
        /// Gets whether the Path is a blank string or not.
        /// </summary>
        /// <returns><see langword="true"/> when the Path is a blank string; otherwise <see langword="false"/>.</returns>
        public bool PathIsEmpty => Path.Length == 0;
        #endregion Properties

        #region Methods
        internal StreamWriter? GetWriter(FileMode fileMode, FileAccess fileAccess, FileShare fileShare) => !this.IsWritingEnabled || this.Path.Length == 0 ? null : new StreamWriter(File.Open(this.Path, fileMode, fileAccess, fileShare));
        /// <summary>
        /// Gets a new <see cref="StreamWriter"/> instance for the file. The caller is responsible for disposing of it.
        /// </summary>
        /// <returns>A new <see cref="StreamWriter"/> instance when this endpoint is enabled; otherwise <see langword="null"/>.</returns>
        public StreamWriter GetStreamWriter() => !this.IsWritingEnabled || this.Path.Length == 0
                ? null!
                : this.GetWriter(FileMode.Append, FileAccess.Write, FileShare.ReadWrite)!;
        /// <inheritdoc/>
        public override TextWriter GetTextWriter() => GetStreamWriter();
        /// <inheritdoc/>
        public override void Reset()
        {
            if (!this.IsWritingEnabled || !File.Exists(this.Path))
                return;
            File.Open(this.Path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite).Dispose();
        }
        #endregion Methods
    }
}