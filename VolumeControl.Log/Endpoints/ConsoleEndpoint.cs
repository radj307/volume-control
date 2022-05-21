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

        /// <inheritdoc/>
        public bool Enabled { get; set; }

        /// <inheritdoc/>
        public TextReader? GetReader() => Console.In;
        /// <inheritdoc/>
        public TextWriter? GetWriter() => Console.Out;
        /// <inheritdoc/>
        public int? ReadRaw() => Console.Read();
        /// <inheritdoc/>
        public string? ReadRawLine() => Console.ReadLine();
        /// <inheritdoc/>
        public void Reset() => Console.Clear();
        /// <inheritdoc/>
        public void WriteRaw(string? str) => Console.Write(str);
        /// <inheritdoc/>
        public void WriteRawLine(string? str = null) => Console.WriteLine(str);
    }
}
