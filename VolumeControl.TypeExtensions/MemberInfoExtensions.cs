using System.Reflection;

namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extends the <see cref="MemberInfo"/> class with additional methods.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the value of the member represented by <paramref name="mInfo"/> from <paramref name="objectInstance"/>.
        /// </summary>
        /// <param name="mInfo"><see cref="MemberInfo"/></param>
        /// <param name="objectInstance">An instance of the object that contains the member represented by <paramref name="mInfo"/>, or <see langword="null"/> if the object is <see langword="static"/>.</param>
        /// <returns>The value of the member, or <see langword="null"/> if <paramref name="mInfo"/> isn't associated with a field or property.</returns>
        public static object? GetValue(this MemberInfo mInfo, object? objectInstance) => mInfo.MemberType switch
        {
            MemberTypes.Field => ((FieldInfo)mInfo).GetValue(objectInstance),
            MemberTypes.Property => ((PropertyInfo)mInfo).GetValue(objectInstance),
            _ => null
        };
        /// <summary>
        /// Sets the value of the member represented by <paramref name="mInfo"/> in <paramref name="objectInstance"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="mInfo"><see cref="MemberInfo"/></param>
        /// <param name="objectInstance">An instance of the object that contains the member represented by <paramref name="mInfo"/>, or <see langword="null"/> if the object is <see langword="static"/>.</param>
        /// <param name="value">The value to set this member to.</param>
        /// <returns><see langword="true"/> when the value of this member was set successfully; <see langword="false"/> when <paramref name="mInfo"/> doesn't represent a field or property.</returns>
        public static bool SetValue(this MemberInfo mInfo, object? objectInstance, object? value)
        {
            switch (mInfo.MemberType)
            {
            case MemberTypes.Field:
                ((FieldInfo)mInfo).SetValue(objectInstance, value);
                return true;
            case MemberTypes.Property:
                ((PropertyInfo)mInfo).SetValue(objectInstance, value);
                return true;
            default:
                return false;
            }
        }
    }
}
