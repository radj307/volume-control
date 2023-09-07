using System.Reflection;

namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extends the <see cref="Type"/> class with additional methods.
    /// </summary>
    public static class TypeExtensions
    {
        /// <inheritdoc cref="Type.IsSubclassOf(Type)"/>
        public static bool IsDerivedFrom(this Type t, Type other) => t.IsSubclassOf(other);
        /// <returns><see langword="true"/> when <paramref name="t"/> is a <see langword="struct"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsStruct(this Type t) => t.IsValueType && !t.IsEnum;
        /// <returns><see langword="true"/> when <paramref name="t"/> is a <see langword="struct"/> <b>and</b> not a primative type <i>(i.e. <see cref="int"/>, <see cref="bool"/>, etc.)</i>; otherwise <see langword="false"/>.</returns>
        public static bool IsCustomStruct(this Type t) => t.IsStruct() && !t.IsPrimitive;
        /// <summary>
        /// Gets all of the types derived from <paramref name="type"/> that are found in the same assembly.
        /// </summary>
        /// <param name="type">A <see cref="Type"/>.</param>
        /// <returns>A list of all types derived from <paramref name="type"/> that are defined in the same assembly.</returns>
        public static IEnumerable<Type> GetAllDerivedTypes(this Type type)
        {
            return Assembly.GetAssembly(type)?.GetAllDerivedTypes(type)!;
        }
        /// <inheritdoc cref="GetAllDerivedTypes(Type)"/>
        public static IEnumerable<Type> GetAllDerivedTypes(this Assembly asm, Type type)
        {
            return asm.GetTypes().Where(t => t != type && type.IsAssignableFrom(t));
        }
    }
}
