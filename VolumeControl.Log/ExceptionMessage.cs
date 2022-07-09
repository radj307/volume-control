using System.Collections;

namespace VolumeControl.Log
{
    /// <summary>
    /// Simple wrapper class for pretty-printing <see cref="Exception"/> messages with <see cref="LogWriter"/>.
    /// </summary>
    public class ExceptionMessage : MessageWrapperBase
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ExceptionMessage"/> instance.
        /// </summary>
        /// <param name="msg">Shown as a prefix on the same line as the <see cref="Exception.Message"/>.</param>
        /// <param name="ex">The <see cref="System.Exception"/> instance to print.</param>
        public ExceptionMessage(string? msg, Exception ex)
        {
            Message = msg;
            Exception = ex;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// The exception associated with this class.
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Shown as a prefix on the same line as the <see cref="Exception.Message"/>.
        /// </summary>
        public new string? Message { get; set; }
        #endregion Properties

        #region Methods
        // gets the formatted message prefix
        private string GetPrefix()
        {
            if (Message is string msg && msg.Length > 0)
            {
                msg = msg.Trim();
                if (!char.IsPunctuation(msg.Last()))
                    msg += ':';
                return msg;
            }
            else return string.Empty;
        }
        /// <inheritdoc/>
        protected override IEnumerable GetMessage() => ToEnumerable($"{GetPrefix()} {LogWriter.FormatExceptionMessage(Exception)}");
        #endregion Methods
    }
}
