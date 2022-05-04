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
        public bool Enabled { get; set; }
        public string Path { get; set; }
        #endregion Properties

        #region Methods
        internal StreamReader? GetReader(FileStreamOptions open)
        {
            if (!Enabled)
                return null;
            return new(File.Open(Path, open));
        }
        internal StreamWriter? GetWriter(FileStreamOptions open)
        {
            if (!Enabled)
                return null;
            return new(File.Open(Path, open)) { AutoFlush = true };
        }
        public StreamReader? GetReader()
        {
            if (!Enabled)
                return null;
            return GetReader(new() { Mode = FileMode.Open, Access = FileAccess.Read, Share = FileShare.ReadWrite });
        }
        public StreamWriter? GetWriter()
        {
            if (!Enabled)
                return null;
            return GetWriter(new() { Mode = FileMode.OpenOrCreate, Access = FileAccess.Write, Share = FileShare.ReadWrite });
        }

        public void WriteRaw(string? str, FileMode mode = FileMode.Append)
        {
            if (!Enabled)
                return;
            using StreamWriter w = new(File.Open(Path, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.Write(str);
            w.Close();
        }
        public void WriteRawLine(string? str = null, FileMode mode = FileMode.Append)
        {
            if (!Enabled)
                return;
            using StreamWriter w = new(File.Open(Path, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.WriteLine(str);
            w.Close();
        }
        public int? ReadRaw(FileMode mode = FileMode.Open)
        {
            if (!Enabled)
                return null;
            using StreamReader r = new(File.Open(Path, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.Read();
        }
        public string? ReadRawLine(FileMode mode = FileMode.Open)
        {
            if (!Enabled)
                return null;
            using StreamReader r = new(File.Open(Path, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.ReadLine();
        }
        public void Reset()
        {
            if (!Enabled || !File.Exists(Path))
                return;
            File.Open(Path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite).Close();
        }

        #endregion Methods
    }
}