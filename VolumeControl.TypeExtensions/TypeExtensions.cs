using System.Reflection;
using System.Runtime.CompilerServices;

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

        public static IEnumerable<Type> GetAllDerivedTypes(this Type type)
        {
            return Assembly.GetAssembly(type)?.GetAllDerivedTypes(type)!;
        }
        public static IEnumerable<Type> GetAllDerivedTypes(this Assembly asm, Type type)
        {
            return asm.GetTypes().Where(t => t != type && type.IsAssignableFrom(t));
        }
    }
}
