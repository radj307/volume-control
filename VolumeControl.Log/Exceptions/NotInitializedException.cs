namespace VolumeControl.Log.Exceptions
{
    /// <summary>
    /// Represents errors that occur as a result of trying to access an object before it is initialized.
    /// </summary>
    public class NotInitializedException : Exception
    {
        #region Constructors
        internal NotInitializedException(string? objectName, string? message, Exception? innerException) : base(message, innerException)
        {
            ObjectName = objectName;
        }
        internal NotInitializedException(string? objectName, string? message) : base(message)
        {
            ObjectName = objectName;
        }
        internal NotInitializedException(string? objectName) : base()
        {
            ObjectName = objectName;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the name of the object that wasn't initialized.
        /// </summary>
        public string? ObjectName { get; }
        #endregion Properties
    }
}
