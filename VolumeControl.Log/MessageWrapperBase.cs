using System.Collections;
using VolumeControl.Log.Enum;

namespace VolumeControl.Log
{
    /// <summary>
    /// Base <see langword="abstract"/> class that implements <see cref="IEnumerable{T}"/> in order to introduce special handling for certain log messages.
    /// </summary>
    public abstract class MessageWrapperBase : IEnumerable
    {
        #region Constructor
        /// <summary>Creates a new <see cref="MessageWrapperBase"/>-derived instance.</summary>
        protected MessageWrapperBase() => _message = new(GetMessage, true);
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the lazily-initialized message value returned by <see cref="GetMessage"/>.
        /// </summary>
        /// <remarks>Because this uses lazy initialization, this only calls <see cref="GetMessage"/> once.</remarks>
        internal IEnumerable Message => _message.Value;
        private readonly Lazy<IEnumerable> _message;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Converts non-enumerable messages to <see cref="IEnumerable"/> messages.
        /// </summary>
        /// <remarks>This method is a convenience wrapper for creating a new array and returning that.</remarks>
        /// <param name="objects">Any number of objects.</param>
        /// <returns><see cref="IEnumerable"/></returns>
        protected static IEnumerable ToEnumerable(params object?[] objects) => objects;
        /// <summary>
        /// Gets the final, printable log message as a string array.
        /// </summary>
        /// <returns>Each line of the log message as a string array.</returns>
        protected abstract IEnumerable GetMessage();
        /// <inheritdoc/>
        public IEnumerator GetEnumerator() => this.Message.GetEnumerator();
        #endregion Methods
    }
}
