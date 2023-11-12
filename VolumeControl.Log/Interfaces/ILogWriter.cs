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
        /// Writes the specified <paramref name="logMessage"/> to the log, if its event type is enabled by the filter.
        /// </summary>
        /// <remarks>
        /// When IsAsyncEnabled is <see langword="true"/>, the message is written asynchronously;
        /// otherwise, the message is written synchronously and the caller will be blocked until the message has been written.
        /// </remarks>
        /// <param name="logMessage">The message to write to the log.</param>
        /// <returns><see langword="true"/> when the <paramref name="logMessage"/>'s event type is enabled; otherwise, <see langword="false"/>.</returns>
        bool LogMessage(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.TRACE"/> log message.
        /// </summary>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Trace(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.DEBUG"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        bool Debug(params object?[] lines);
        /// <summary>
        /// Queues writing an <see cref="EventType.DEBUG"/> log message.
        /// </summary>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Debug(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.INFO"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        bool Info(params object?[] lines);
        /// <summary>
        /// Queues writing an <see cref="EventType.INFO"/> log message.
        /// </summary>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Info(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.WARN"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        bool Warning(params object?[] lines);
        /// <summary>
        /// Queues writing an <see cref="EventType.WARN"/> log message.
        /// </summary>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Warning(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.ERROR"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        bool Error(params object?[] lines);
        /// <summary>
        /// Queues writing an <see cref="EventType.ERROR"/> log message.
        /// </summary>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Error(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.FATAL"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        bool Fatal(params object?[] lines);
        /// <summary>
        /// Queues writing an <see cref="EventType.FATAL"/> log message.
        /// </summary>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Fatal(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.CRITICAL"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        bool Critical(params object?[] lines);
        /// <summary>
        /// Queues writing an <see cref="EventType.CRITICAL"/> log message.
        /// </summary>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Critical(LogMessage logMessage);
        /// <summary>
        /// Queues writing an <see cref="EventType.NONE"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <remarks>
        /// This produces a message with no event type header, but a timestamp is still shown.
        /// </remarks>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        bool Blank(params object?[] lines);
        /// <summary>
        /// Queues writing an <see cref="EventType.NONE"/> log message.
        /// </summary>
        /// <remarks>
        /// This produces a message with no event type header, but a timestamp is still shown.
        /// </remarks>
        /// <param name="logMessage">A <see cref="Log.LogMessage"/> instance to write.</param>
        bool Blank(LogMessage logMessage);
    }
}
