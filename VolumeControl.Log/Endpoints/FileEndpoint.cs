namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// Log endpoint that allows writing logs directly to a file on disk.
    /// </summary>
    public class FileEndpoint : IEndpoint
    {
        #region Constructors
        /// <inheritdoc cref="FileEndpoint"/>
        /// <param name="path">The location of the log file.</param>
        /// <param name="enabled">Whether the endpoint is already enabled when constructed or not.</param>
        public FileEndpoint(string path, bool enabled)
        {
            this.Path = path;
            this.Enabled = enabled;
        }
        #endregion Constructors

        #region Properties
        /// <inheritdoc/>
        public bool Enabled { get; set; }
        /// <summary>
        /// The location of the log file.
        /// </summary>
        public string Path { get; set; }
        #endregion Properties

        #region Methods
        internal TextReader? GetReader(FileStreamOptions open) => !this.Enabled || this.Path.Length == 0 ? null : (TextReader)new StreamReader(File.Open(this.Path, open));
        internal TextWriter? GetWriter(FileStreamOptions open) => !this.Enabled || this.Path.Length == 0 ? null : (TextWriter)new StreamWriter(File.Open(this.Path, open)) { AutoFlush = true };
        /// <inheritdoc/>
        public TextReader? GetReader() => !this.Enabled || this.Path.Length == 0
                ? null
                : this.GetReader(new() { Mode = FileMode.Open, Access = FileAccess.Read, Share = FileShare.ReadWrite });
        /// <inheritdoc/>
        public TextWriter? GetWriter() => !this.Enabled || this.Path.Length == 0
                ? null
                : this.GetWriter(new() { Mode = FileMode.Append, Access = FileAccess.Write, Share = FileShare.ReadWrite });

        /// <inheritdoc/>
        public void WriteRaw(string? str, FileMode mode)
        {
            if (!this.Enabled || this.Path.Length == 0)
                return;
            using StreamWriter w = new(File.Open(this.Path, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.Write(str);
            w.Close();
        }
        /// <inheritdoc/>
        public void WriteRaw(string? str) => this.WriteRaw(str, FileMode.Append);
        /// <inheritdoc/>
        public void WriteRawLine(string? str, FileMode mode)
        {
            if (!this.Enabled || this.Path.Length == 0)
                return;
            using StreamWriter w = new(File.Open(this.Path, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.WriteLine(str);
            w.Close();
        }
        /// <inheritdoc/>
        public void WriteRawLine(string? str) => this.WriteRawLine(str, FileMode.Append);
        /// <inheritdoc/>
        public int? ReadRaw(FileMode mode)
        {
            if (!this.Enabled || this.Path.Length == 0)
                return null;
            using StreamReader r = new(File.Open(this.Path, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.Read();
        }
        /// <inheritdoc/>
        public string? ReadRawLine(FileMode mode)
        {
            if (!this.Enabled || this.Path.Length == 0)
                return null;
            using StreamReader r = new(File.Open(this.Path, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.ReadLine();
        }
        /// <inheritdoc/>
        public void Reset()
        {
            if (!this.Enabled || !File.Exists(this.Path))
                return;
            File.Open(this.Path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite).Close();
        }

        /// <inheritdoc/>
        public int? ReadRaw() => this.ReadRaw(FileMode.Open);
        /// <inheritdoc/>
        public string? ReadRawLine() => this.ReadRawLine(FileMode.Open);

        #endregion Methods
    }
}