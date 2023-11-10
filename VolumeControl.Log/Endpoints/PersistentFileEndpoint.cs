using System.Text;

namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// Log endpoint that allows writing logs directly to a file on disk, and keeps the file stream open.
    /// </summary>
    public class PersistentFileEndpoint : FileEndpoint, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="PersistentFileEndpoint"/> instance.
        /// </summary>
        /// <param name="path">The path to the output file.</param>
        /// <param name="enabled">Whether the endpoint starts enabled or not.</param>
        public PersistentFileEndpoint(string path, bool enabled) : base(path, enabled)
        {
            CreateWriter();
        }
        #endregion Constructor

        #region (class) FileEndpointWriter
        class FileEndpointWriter : StreamWriter, IDisposable
        {
            #region Constructors
            public FileEndpointWriter(Stream stream) : base(stream) { }
            public FileEndpointWriter(string path) : base(path) { }
            public FileEndpointWriter(Stream stream, Encoding encoding) : base(stream, encoding) { }
            public FileEndpointWriter(string path, FileStreamOptions options) : base(path, options) { }
            public FileEndpointWriter(string path, bool append) : base(path, append) { }
            public FileEndpointWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream, encoding, bufferSize) { }
            public FileEndpointWriter(string path, bool append, Encoding encoding) : base(path, append, encoding) { }
            public FileEndpointWriter(string path, Encoding encoding, FileStreamOptions options) : base(path, encoding, options) { }
            public FileEndpointWriter(Stream stream, Encoding? encoding = null, int bufferSize = -1, bool leaveOpen = false) : base(stream, encoding, bufferSize, leaveOpen) { }
            public FileEndpointWriter(string path, bool append, Encoding encoding, int bufferSize) : base(path, append, encoding, bufferSize) { }
            #endregion Constructors

            #region Fields
            internal readonly object _lock = new();
            #endregion Fields

            #region Properties
            /// <inheritdoc/>
            public override Encoding Encoding => Encoding.UTF8;
            /// <summary>
            /// Gets or sets whether this <see cref="FileEndpointWriter"/> instance can be closed and disposed of or not.
            /// </summary>
            internal bool CanClose
            {
                get => _canClose;
                set
                {
                    lock (_lock)
                    {
                        _canClose = value;
                    }
                }
            }
            private bool _canClose = false;
            #endregion Properties

            #region IDisposable Override
            /// <summary>
            /// Disposes of the <see cref="FileEndpointWriter"/> instance.
            /// </summary>
            ~FileEndpointWriter()
            { // we're out of scope, allow disposal
                CanClose = true;
                Dispose();
            }
            /// <summary>
            /// Does nothing unless CanClose is <see langword="true"/>.
            /// </summary>
            public override void Close()
            {
                if (!CanClose) return;
                base.Close();
            }
            /// <summary>
            /// Does nothing unless CanClose is <see langword="true"/>.
            /// </summary>
            public new void Dispose()
            {
                try
                {
                    Flush(); //< calls to Dispose() may expect that the stream is flushed
                }
                catch { }
                if (!CanClose) return;
                base.Dispose();
                GC.SuppressFinalize(this);
            }
            /// <summary>
            /// Does nothing unless CanClose is <see langword="true"/>.
            /// </summary>
            public override ValueTask DisposeAsync()
            {
                FlushAsync().GetAwaiter().GetResult(); //< calls to DisposeAsync() may expect that the stream is flushed
                if (!CanClose) return ValueTask.CompletedTask;
                GC.SuppressFinalize(this);
                return base.DisposeAsync();
            }
            #endregion IDisposable Override

            #region Write Override
            public override void Write(char value) //< this is used by all other write methods
            {
                lock (_lock)
                {
                    base.Write(value);
                }
            }
            #endregion Write Override
        }
        #endregion (class) FileEndpointWriter

        #region Fields
        private FileEndpointWriter _writer = null!;
        #endregion Fields

        #region Methods

        #region (Private)
        private void DeleteWriter()
        {
            lock (_writer._lock)
            {
                _writer.CanClose = true;
                _writer.Dispose();
            }
            _writer = null!;
        }
        private void CreateWriter(FileMode fileMode = FileMode.OpenOrCreate)
        {
            if (_writer != null) throw new InvalidOperationException();
            _writer = new(File.Open(Path, fileMode, FileAccess.Write, FileShare.Read), Encoding.UTF8);
        }
        #endregion (Private)

        /// <remarks>
        /// The writer cannot be disposed of externally, even when calling Dispose().
        /// </remarks>
        /// <inheritdoc/>
        public override TextWriter GetTextWriter() => _writer;
        /// <summary>
        /// Gets the persistent stream writer instance.
        /// </summary>
        /// <returns>The persistent <see cref="StreamWriter"/> instance.</returns>
        public override StreamWriter GetStreamWriter() => _writer;
        /// <inheritdoc/>
        public override void Reset()
        {
            DeleteWriter();
            CreateWriter(FileMode.Truncate);
        }
        #endregion Methods

        #region IDisposable
        /// <summary>
        /// Disposes of this <see cref="PersistentFileEndpoint"/> &amp; its underlying stream writer instance.
        /// </summary>
        ~PersistentFileEndpoint() => Dispose();
        /// <inheritdoc/>
        public void Dispose()
        {
            _writer.CanClose = true;
            _writer.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}