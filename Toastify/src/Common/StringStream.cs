using System.IO;
using System.Text;

namespace Toastify.Common
{
    public class StringStream
    {
        private readonly Stream ioStream;
        private readonly UnicodeEncoding streamEncoding;

        public StringStream(Stream ioStream)
        {
            this.ioStream = ioStream;
            this.streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int length = this.ioStream.ReadByte() * 256;
            length += this.ioStream.ReadByte();
            byte[] inBuffer = new byte[length];
            this.ioStream.Read(inBuffer, 0, length);

            return this.streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = this.streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > ushort.MaxValue)
                len = ushort.MaxValue;

            this.ioStream.WriteByte((byte)(len / 256));
            this.ioStream.WriteByte((byte)(len & 255));
            this.ioStream.Write(outBuffer, 0, len);
            this.ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}