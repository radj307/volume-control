namespace VolumeControl.Log
{
    public class FileEndpoint : IEndpoint
    {
        #region Constructors
        public FileEndpoint(string path)
        {
            Path = path;
        }
        #endregion Constructors

        #region Members
        private string? _filepath = null;
        #endregion Members

        #region Properties
        public bool Ready
        {
            get => _filepath != null;
        }
        public string? Path
        {
            get => _filepath;
            set => _filepath = value;
        }
        #endregion Properties

        #region Methods
        protected StreamWriter GetWriter(FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
            => new(File.Open(_filepath!, FileMode.OpenOrCreate, access, share)) { AutoFlush = true };

        public void WriteRaw(string? str, FileMode mode = FileMode.Append)
        {
            if (Ready)
            {
                using StreamWriter w = new(File.Open(_filepath!, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
                w.Write(str);
                w.Close();
            }
        }

        public void WriteRawLine(string? str = null, FileMode mode = FileMode.Append)
        {
            if (Ready)
            {
                using StreamWriter w = new(File.Open(_filepath!, mode, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
                w.WriteLine(str);
                w.Close();
            }
        }

        public int? ReadRaw(FileMode mode = FileMode.Open)
        {
            if (Ready)
            {
                using StreamReader r = new(File.Open(_filepath!, mode, FileAccess.Read, FileShare.ReadWrite));
                return r.Read();
            }
            return null;
        }

        public string? ReadRawLine(FileMode mode = FileMode.Open)
        {
            if (Ready)
            {
                using StreamReader r = new(File.Open(_filepath!, mode, FileAccess.Read, FileShare.ReadWrite));
                return r.ReadLine();
            }
            return null;
        }

        public void Reset()
        {
            File.Open(_filepath!, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite).Close();
        }
        #endregion Methods
    }
}