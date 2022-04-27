namespace VolumeControl.Log.Endpoints
{
    public class FileEndpoint : IEndpoint
    {
        #region Constructors
        public FileEndpoint()
        {
        }
        public FileEndpoint(string path)
        {
            Path = path;
        }
        #endregion Constructors

        #region Members
        private string? _filepath = null;
        #endregion Members

        #region Properties
        public bool Enabled { get; set; }
        public string? Path
        {
            get => _filepath;
            set => _filepath = value;
        }
        #endregion Properties

        #region Methods
        internal StreamReader? GetReader(FileStreamOptions open)
        {
            if (!Enabled)
                return null;
            return new(File.Open(_filepath!, open));
        }
        internal StreamWriter? GetWriter(FileStreamOptions open)
        {
            if (!Enabled)
                return null;
            return new(File.Open(_filepath!, open)) { AutoFlush = true };
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
            using StreamWriter w = new(File.Open(_filepath!, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.Write(str);
            w.Close();
        }
        public void WriteRawLine(string? str = null, FileMode mode = FileMode.Append)
        {
            if (!Enabled)
                return;
            using StreamWriter w = new(File.Open(_filepath!, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
            w.WriteLine(str);
            w.Close();
        }
        public int? ReadRaw(FileMode mode = FileMode.Open)
        {
            if (!Enabled)
                return null;
            using StreamReader r = new(File.Open(_filepath!, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.Read();
        }
        public string? ReadRawLine(FileMode mode = FileMode.Open)
        {
            if (!Enabled)
                return null;
            using StreamReader r = new(File.Open(_filepath!, mode, FileAccess.Read, FileShare.ReadWrite));
            return r.ReadLine();
        }
        public void Reset()
        {
            if (!Enabled)
                return;
            File.Open(_filepath!, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite).Close();
        }

        #endregion Methods
    }
}