namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// Wrapper endpoint for <see cref="Console"/>.
    /// </summary>
    public class ConsoleEndpoint : IEndpoint
    {
        public ConsoleEndpoint()
        {
            Enabled = true;
        }

        public bool Enabled { get; set; }

        public TextReader? GetReader() => Console.In;
        public TextWriter? GetWriter() => Console.Out;
        public int? ReadRaw() => Console.Read();
        public string? ReadRawLine() => Console.ReadLine();
        public void Reset() => Console.Clear();
        public void WriteRaw(string? str) => Console.Write(str);
        public void WriteRawLine(string? str = null) => Console.WriteLine(str);
    }
}
