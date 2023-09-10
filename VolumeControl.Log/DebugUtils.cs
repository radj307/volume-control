using System.Runtime.CompilerServices;

namespace VolumeControl.Log
{
    /// <summary>
    /// Utility helper functions for writing log messages that are more helpful for debugging.
    /// </summary>
    public static class DebugUtils
    {
        /// <summary>
        /// Gets the filename that this function was called from.
        /// </summary>
        /// <param name="_filePath"><b>Do not provide a value for this, or the function will not work correctly!</b></param>
        /// <returns>The filename of the code file where this function was called.</returns>
        public static string GetCurrentFileName([CallerFilePath] string _filePath = "")
            => _filePath;
        /// <summary>
        /// Gets the line number in the source file that this function was called from.
        /// </summary>
        /// <param name="_line"><b>Do not provide a value for this, or the function will not work correctly!</b></param>
        /// <returns>The line number in the source file that this function was called from, or -1 if an error occurred.</returns>
        public static int GetCurrentLineNumber([CallerLineNumber] int _line = -1)
            => _line;
        /// <summary>
        /// Gets the name of the member or property that this function was called from.
        /// </summary>
        /// <param name="_memberName"><b>Do not provide a value for this, or the function will not work correctly!</b></param>
        /// <returns>The name of the member or property that this function was called from.</returns>
        public static string GetCurrentMemberName([CallerMemberName] string _memberName = "")
            => _memberName;
    }
}
