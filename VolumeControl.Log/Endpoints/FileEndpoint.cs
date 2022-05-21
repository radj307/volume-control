namespace VolumeControl.Log.Endpoints
{
    public class FileEndpoint : IEndpoint
    {
        #region Constructors
        public FileEndpoint(string path, bool enabled)
        {
            Path = path;
            Enabled = enabled;
        }
        #endregion Constructors

        #region Properties
        /// <inheritdoc/>
        public bool Enabled { get; set; }
        public string Path { get; set; }
        #endregion Properties

        #region Methods
        internal TextReader? GetReader(FileStreamOptions open)
        {
            if (!Enabled || Path.Length == 0)
                return null;
            return new StreamReader(File.Open(Path, open));
        }
        internal TextWriter? GetWriter(FileStreamOptions open)
        {
            if (!Enabled || Path.Length == 0)
                return null;
            return new StreamWriter(File.Open(Path, open)) { AutoFlush = true };
        }
        /// <inheritdoc/>
        public TextReader? GetReader()
        {
            if (!Enabled || Path.Length == 0)
                return null;
            return GetReader(new() { Mode = FileMode.Open, Access = FileAccess.Read, Share = FileShare.ReadWrite });
        }
        /// <inheritdoc/>
        public TextWriter? GetWriter()
        {
            if (!Enabled || Path.Length == 0)
                return null;
            return GetWriter(new() { Mode = FileMode.Append, Access = FileAccess.Write, Share = FileShare.ReadWrite });
        }

        /// <inheritdoc/>
        public void WriteRaw(string? str, FileMode mode)
        {
            if (!Enabled || Path.Length == 0)
                return;
            using StreamWriter w = new(File.Open(Path, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.Write(str);
            w.Close();
        }
        /// <inheritdoc/>
        public void WriteRaw(string? str) => WriteRaw(str, FileMode.Append);
        /// <inheritdoc/>
        public void WriteRawLine(string? str, FileMode mode)
        {
            if (!Enabled || Path.Length == 0)
                return;
            using StreamWriter w = new(File.Open(Path, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.WriteLine(str);
            w.Close();
        }
        /// <inheritdoc/>
        public void WriteRawLine(string? str) => WriteRawLine(str, FileMode.Append);
        /// <inheritdoc/>
        public int? ReadRaw(FileMode mode)
        {
            if (!Enabled || Path.Length == 0)
                return null;
            using StreamReader r = new(File.Open(Path, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.Read();
        }
        /// <inheritdoc/>
        public string? ReadRawLine(FileMode mode)
        {
            if (!Enabled || Path.Length == 0)
                return null;
            using StreamReader r = new(File.Open(Path, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.ReadLine();
        }
        /// <inheritdoc/>
        public void Reset()
        {
            if (!Enabled || !File.Exists(Path))
                return;
            File.Open(Path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite).Close();
        }

        /// <inheritdoc/>
        public int? ReadRaw() => ReadRaw(FileMode.Open);
        /// <inheritdoc/>
        public string? ReadRawLine() => ReadRawLine(FileMode.Open);

        #endregion Methods
    }
}