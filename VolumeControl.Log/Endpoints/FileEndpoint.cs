using System.Text;
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
            _fileInfo = new(path);
            _path = path;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the file info object for the log file.
        /// </summary>
        public FileInfo FileInfo
        {
            get => _fileInfo;
            set
            {
                if (value == _fileInfo) return;

                _fileInfo = value;
                _path = _fileInfo.FullName;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Path));
            }
        }
        private FileInfo _fileInfo;
        /// <summary>
        /// Gets or sets the location of the log file.
        /// </summary>
        public string Path
        {
            get => _path;
            set
            {
                if (value.Equals(_path, StringComparison.Ordinal)) return;

                _path = value;
                _fileInfo = new(_path);
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FileInfo));
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
        /// <summary>
        /// Gets a new <see cref="StreamWriter"/> instance for the file. The caller is responsible for disposing of it.
        /// </summary>
        /// <returns>A new <see cref="StreamWriter"/> instance when this endpoint is enabled; otherwise <see langword="null"/>.</returns>
        public virtual StreamWriter GetStreamWriter() => !this.IsWritingEnabled || this.Path.Length == 0
                ? null!
                : new(FileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8)!;
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