namespace VolumeControl.Log.Endpoints
{
    public class FileEndpoint : IEndpoint
    {
        #region Constructors
        public FileEndpoint() { }
        public FileEndpoint(string path)
        {
            Path = path;
        }
        #endregion Constructors

        #region Members
        private string? _filepath = null;
        private bool _enabled = Properties.Settings.Default.EnableLog;
        #endregion Members

        #region Properties
        public bool Ready => _filepath != null;
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }
        public string? Path
        {
            get => _filepath;
            set => _filepath = value;
        }
        #endregion Properties

        #region Methods
        internal StreamReader? GetReader(FileStreamOptions open)
            => Enabled ? new(File.Open(_filepath!, open)) : null;
        internal StreamWriter? GetWriter(FileStreamOptions open)
            => Enabled ? new(File.Open(_filepath!, open)) { AutoFlush = true } : null;
        public StreamReader? GetReader() => GetReader(new() { Mode = FileMode.Open, Access = FileAccess.Read, Share = FileShare.ReadWrite });
        public StreamWriter? GetWriter() => GetWriter(new() { Mode = FileMode.OpenOrCreate, Access = FileAccess.Write, Share = FileShare.ReadWrite });
        public void WriteRaw(string? str, FileMode mode = FileMode.Append)
        {
            if (Ready && Enabled)
            {
                using StreamWriter w = new(File.Open(_filepath!, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
                w.Write(str);
                w.Close();
            }
        }
        public void WriteRawLine(string? str = null, FileMode mode = FileMode.Append)
        {
            if (Ready && Enabled)
            {
                using StreamWriter w = new(File.Open(_filepath!, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
                w.WriteLine(str);
                w.Close();
            }
        }
        public int? ReadRaw(FileMode mode = FileMode.Open)
        {
            if (Ready && Enabled)
            {
                using StreamReader r = new(File.Open(_filepath!, mode, FileAccess.Read, FileShare.ReadWrite));
                return r.Read();
            }
            return null;
        }
        public string? ReadRawLine(FileMode mode = FileMode.Open)
        {
            if (Ready && Enabled)
            {
                using StreamReader r = new(File.Open(_filepath!, mode, FileAccess.Read, FileShare.ReadWrite));
                return r.ReadLine();
            }
            return null;
        }
        public void Reset()
        {
            if (Enabled)
                File.Open(_filepath!, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite).Close();
        }

        #endregion Methods
    }
}