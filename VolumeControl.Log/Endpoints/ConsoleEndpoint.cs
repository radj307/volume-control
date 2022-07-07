namespace VolumeControl.Log.Endpoints
{
    /// <summary>
    /// Allows using the <see cref="Console"/> as a logging endpoint.
    /// </summary>
    public class ConsoleEndpoint : IEndpoint
    {
        /// <inheritdoc cref="ConsoleEndpoint"/>
        public ConsoleEndpoint() => this.Enabled = true;

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
