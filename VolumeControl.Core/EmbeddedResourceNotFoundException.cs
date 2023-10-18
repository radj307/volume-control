using System.Reflection;

namespace VolumeControl.Core
{
    /// <summary>
    /// Represents errors that occur while trying to access an embedded resource file.
    /// </summary>
    public class EmbeddedResourceNotFoundException : Exception
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="EmbeddedResourceNotFoundException"/> instance with the specified <paramref name="message"/> and <paramref name="innerException"/>.
        /// </summary>
        /// <param name="resourceName">The name of the resource that wasn't found.</param>
        /// <param name="sourceAssembly">The assembly that the resource wasn't found in.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing"/> in Visual Basic) if no inner exception is specified.</param>
        public EmbeddedResourceNotFoundException(string resourceName, Assembly sourceAssembly, string? message, Exception? innerException)
            : base(message, innerException)
        {
            ResourceName = resourceName;
            SourceAssembly = sourceAssembly;
        }
        /// <summary>
        /// Creates a new <see cref="EmbeddedResourceNotFoundException"/> instance with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="resourceName">The name of the resource that wasn't found.</param>
        /// <param name="sourceAssembly">The assembly that the resource wasn't found in.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public EmbeddedResourceNotFoundException(string resourceName, Assembly sourceAssembly, string? message)
            : base(message)
        {
            ResourceName = resourceName;
            SourceAssembly = sourceAssembly;
        }
        /// <summary>
        /// Creates a new <see cref="EmbeddedResourceNotFoundException"/> instance with the specified <paramref name="innerException"/> and a default message generated from the specified <paramref name="resourceName"/> and <paramref name="sourceAssembly"/>.
        /// </summary>
        /// <param name="resourceName">The name of the resource that wasn't found.</param>
        /// <param name="sourceAssembly">The assembly that the resource wasn't found in.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing"/> in Visual Basic) if no inner exception is specified.</param>
        public EmbeddedResourceNotFoundException(string resourceName, Assembly sourceAssembly, Exception? innerException)
            : base(GetDefaultMessage(resourceName, sourceAssembly), innerException)
        {
            ResourceName = resourceName;
            SourceAssembly = sourceAssembly;
        }
        /// <summary>
        /// Creates a new <see cref="EmbeddedResourceNotFoundException"/> instance with a default message generated from the specified <paramref name="resourceName"/> and <paramref name="sourceAssembly"/>.
        /// </summary>
        /// <param name="resourceName">The name of the resource that wasn't found.</param>
        /// <param name="sourceAssembly">The assembly that the resource wasn't found in.</param>
        public EmbeddedResourceNotFoundException(string resourceName, Assembly sourceAssembly)
            : base(GetDefaultMessage(resourceName, sourceAssembly))
        {
            ResourceName = resourceName;
            SourceAssembly = sourceAssembly;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the name of the missing resource file.
        /// </summary>
        public string ResourceName { get; }
        /// <summary>
        /// Gets the <see cref="Assembly"/> that was accessed.
        /// </summary>
        public Assembly SourceAssembly { get; }
        #endregion Properties

        #region Methods
        private static string GetDefaultMessage(string? resourceName, Assembly assembly)
        {
            if (resourceName == null)
                return "The specified resource name was null!";

            string message = $"An embedded resource with the name \"{resourceName}\" was not found in assembly {assembly.FullName}!";
            if (FindActualResourceName(assembly, resourceName) is string actualName)
            {
                message += $" (Did you mean \"{actualName}\"?)";
            }
            else
            {
                message += $" Make sure that the {assembly.FullName} assembly contains the {resourceName} file, and that the Build Action is set to \"Embedded resource\" in the file properties.";
            }
            return message;
        }
        /// <summary>
        /// Searches the specified <paramref name="assembly"/> for an embedded resource with a similar name to <paramref name="resourceName"/>.
        /// </summary>
        /// <remarks>
        /// If the specified <paramref name="resourceName"/> is missing namespace qualifiers, this method will find the correct full name.
        /// When this method returns <see langword="null"/>, it indicates that the <paramref name="resourceName"/> may be spelled wrong, the resource is in a different assembly, or the <b>Build Action</b> for the file isn't set to <see langword="Embedded resource"/>.
        /// </remarks>
        /// <param name="assembly">The <see cref="Assembly"/> to search.</param>
        /// <param name="resourceName">The given name of the resource file.</param>
        /// <param name="stringComparison">The comparison type to use when comparing strings.</param>
        /// <returns>The specified <paramref name="resourceName"/> with namespace qualifiers included if found in the <paramref name="assembly"/>; otherwise <see langword="null"/>.</returns>
        protected static string? FindActualResourceName(Assembly assembly, string resourceName, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            var names = assembly.GetManifestResourceNames();

            string? possibleMatch = null;
            for (int i = 0, max = names.Length; i < max; ++i)
            {
                string name = names[i];
                if (name.EndsWith(resourceName, stringComparison))
                { // resourceName is missing qualifiers
                    return name;
                }
                else if (name.Contains(resourceName, stringComparison))
                { // resourceName may be missing qualifiers and file extension (or this resource just has a similar name)
                    possibleMatch = name;
                }
            }

            return possibleMatch;
        }
        #endregion Methods
    }
}
