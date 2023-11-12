using System.Collections;

namespace VolumeControl.Log
{
    /// <summary>
    /// Represents a message to be written to the log.
    /// </summary>
    public sealed class LogMessage : IEnumerable<object?>, IEnumerable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="LogMessage"/> instance with the specified <paramref name="eventType"/>.
        /// </summary>
        /// <param name="eventType">The <see cref="Log.EventType"/> of this message.</param>
        /// <param name="lines">The contents of the message, where each element represents one "line".</param>
        public LogMessage(EventType eventType, params object?[] lines)
        {
            EventType = eventType;
            Lines = lines.ToList();
        }
        /// <summary>
        /// Creates a new empty <see cref="LogMessage"/> instance with the specified <paramref name="eventType"/>.
        /// </summary>
        /// <param name="eventType">The <see cref="Log.EventType"/> of this message.</param>
        public LogMessage(EventType eventType)
        {
            EventType = eventType;
            Lines = new();
        }
        /// <summary>
        /// Creates a new <see cref="LogMessage"/> instance with the specified <paramref name="lines"/> and an undefined EventType.
        /// </summary>
        /// <param name="lines">The contents of the message, where each element represents one "line".</param>
        public LogMessage(params object?[] lines)
        {
            EventType = EventType.NONE;
            Lines = lines.ToList();
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the event type of this message.
        /// </summary>
        public EventType EventType { get; set; }
        /// <summary>
        /// Gets whether this <see cref="LogMessage"/> instance has an EventType or not.
        /// </summary>
        public bool HasEventType => EventType != EventType.NONE;
        /// <summary>
        /// Gets or sets the lines in this message.
        /// </summary>
        public List<object?> Lines { get; set; }
        /// <summary>
        /// Gets whether there are any lines in this message.
        /// </summary>
        public bool IsEmpty => Lines.Count == 0;
        #endregion Properties

        #region Operators
        /// <summary>
        /// Converts a <see cref="string"/> to a new <see cref="LogMessage"/> instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        public static implicit operator LogMessage(string s) => new(s);
        #endregion Operators

        #region Methods

        #region ToString
        /// <summary>
        /// Gets the contents of the log message as a single string.
        /// </summary>
        public override string ToString() => string.Join(Environment.NewLine, Lines);
        #endregion ToString

        #region Add
        /// <summary>
        /// Appends the specified <paramref name="line"/> to the message.
        /// </summary>
        /// <param name="line">The content of the line.</param>
        /// <returns>This instance.</returns>
        public LogMessage Add(object? line)
        {
            Lines.Add(line);
            return this;
        }
        #endregion Add

        #region SetEventType
        /// <summary>
        /// Pipeline method that sets the event type of this log message.
        /// </summary>
        /// <param name="eventType">The EventType to use for this log message.</param>
        /// <returns>This instance.</returns>
        public LogMessage SetEventType(EventType eventType)
        {
            EventType = eventType;
            return this;
        }
        #endregion SetEventType

        #endregion Methods

        #region IEnumerable
        /// <inheritdoc/>
        public IEnumerator<object?> GetEnumerator() => ((IEnumerable<object?>)this.Lines).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.Lines).GetEnumerator();
        #endregion IEnumerable
    }
}
