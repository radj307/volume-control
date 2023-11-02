namespace VolumeControl.Log.Interfaces
{
    /// <summary>
    /// Represents a log writer instance.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Checks if messages with the specified <paramref name="eventType"/> are shown in the log.
        /// </summary>
        /// <param name="eventType">An <see cref="EventType"/> to check.</param>
        /// <returns><see langword="true"/> when the <paramref name="eventType"/> is shown; otherwise <see langword="false"/>.</returns>
        bool FilterEventType(EventType eventType);
        /// <summary>
        /// Writes the specified <paramref name="logMessage"/> to the log.
        /// </summary>
        /// <remarks>
        /// When IsAsyncEnabled is <see langword="true"/>, the message is added to the queue and written asynchronously; otherwise, the message is blocking and the message is written synchronously.
        /// </remarks>
        /// <param name="logMessage"></param>
        void LogMessage(LogMessage logMessage);
        /// <summary>
        /// Queues writing a <see cref="EventType.TRACE"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Trace(params object?[] lines);
        /// <summary>
        /// Queues writing a <see cref="EventType.DEBUG"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Debug(params object?[] lines);
        /// <summary>
        /// Queues writing a <see cref="EventType.INFO"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Info(params object?[] lines);
        /// <summary>
        /// Queues writing a <see cref="EventType.WARN"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Warning(params object?[] lines);
        /// <summary>
        /// Queues writing a <see cref="EventType.ERROR"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Error(params object?[] lines);
        /// <summary>
        /// Queues writing a <see cref="EventType.FATAL"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Fatal(params object?[] lines);
        /// <summary>
        /// Queues writing a <see cref="EventType.CRITICAL"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Critical(params object?[] lines);
        /// <summary>
        /// Queues writing a <see cref="EventType.NONE"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <remarks>
        /// This produces a message with no event type header, but a timestamp is still shown.
        /// </remarks>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        void Blank(params object?[] lines);
    }
}
