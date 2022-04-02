namespace VolumeControl.Core.Logging
{
    public class Endpoint : IDisposable
    {
        public Endpoint(string outfile, FileAccess access = FileAccess.ReadWrite, FileShare sharing = FileShare.Read)
        {
            _stream = new(outfile, FileMode.OpenOrCreate, access, sharing);
        }

        #region Members
        private readonly FileStream _stream;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        #endregion Members

        #region Properties
        public StreamReader Reader
        {
            get
            {
                if (_reader == null)
                {
                    _reader = new StreamReader(_stream);
                }
                return _reader;
            }
        }
        public StreamWriter Writer
        {
            get
            {
                if (_writer == null)
                {
                    _writer = new StreamWriter(_stream);
                    _writer.AutoFlush = true;
                }
                return _writer;
            }
        }
        #endregion Properties

        #region Methods
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _stream.Dispose();
            _reader?.Dispose();
            _writer?.Dispose();
        }
        #endregion Methods
    }
}
