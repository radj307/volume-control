using VolumeControl.Log.Endpoints;

namespace VolumeControl.Log
{
    public class MemoryEndpoint : IEndpoint
    {
        public MemoryEndpoint(bool enabled = true)
        {
            _stream = new();
            Enabled = enabled;
        }

        private MemoryStream _stream;

        public bool Enabled { get; set; }

        public TextReader? GetReader() => Enabled ? new StreamReader(_stream) : null;
        public TextWriter? GetWriter() => Enabled ? new StreamWriter(_stream) : null;
        public int? ReadRaw()
        {
            if (!Enabled)
                return null;
            using var r = GetReader();
            int? ch = r?.Read();
            r?.Dispose();
            return ch;
        }
        public string? ReadRawLine()
        {
            if (!Enabled)
                return null;
            using var r = GetReader();
            string? line = r?.ReadLine();
            r?.Dispose();
            return line;
        }
        public void Reset() => _stream = new();
        public void WriteRaw(string? str)
        {
            if (!Enabled || str == null)
                return;
            _stream.Write(new Span<byte>(str.ToCharArray().Cast<byte>().ToArray()));
        }
        public void WriteRawLine(string? str = null)
        {
            if (!Enabled)
                return;
            WriteRaw((str == null) ? "\n" : $"{str}\n");
        }
    }
}
