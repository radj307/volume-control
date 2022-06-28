namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extends the <see cref="Type"/> class with additional methods.
    /// </summary>
    public static class TypeExtensions
    {
        /// <inheritdoc cref="Type.IsSubclassOf(Type)"/>
        public static bool IsDerivedFrom(this Type t, Type other) => t.IsSubclassOf(other);
    }
}
