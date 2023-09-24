using System;
using System.Collections.Generic;
using System.Reflection;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Addon
{
    /// <summary>
    /// Wraps an <see cref="Attribute"/> type.
    /// </summary>
    public class AttributeWrapper
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="AttributeWrapper"/> instance for the specified <paramref name="attributeType"/>.
        /// </summary>
        /// <param name="attributeType">The <see cref="System.Type"/> of an <see cref="Attribute"/>-derived type.</param>
        /// <param name="checkInheritedAttributes">When <see langword="true"/>, attributes that were inherited are also checked; when <see langword="false"/>, only attributes that were <b>directly</b> applied to the type are checked.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="attributeType"/> is <b>not</b> derived from <see cref="Attribute"/>!</exception>
        public AttributeWrapper(Type attributeType, bool checkInheritedAttributes = true)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, "Invalid attribute type!");

            Type = attributeType;

            // determine what the attribute type can be applied to
            if (Type.GetCustomAttribute<AttributeUsageAttribute>(true) is AttributeUsageAttribute usageAttr)
                Targets = usageAttr.ValidOn;
            else Targets = AttributeTargets.All;

            CheckInherited = checkInheritedAttributes;
        }
        #endregion Constructor

        /// <summary>
        /// Gets the <see cref="System.Type"/> of the target attribute.
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// Gets the <see cref="AttributeTargets"/> for <see cref="Type"/>.
        /// </summary>
        public AttributeTargets Targets { get; }
        /// <summary>
        /// Gets whether inherited attributes are checked or not.
        /// </summary>
        public bool CheckInherited { get; }

        private static LogWriter Log => FLog.Log;

        /// <summary>
        /// Attempts to load the attribute type from the given <see cref="System.Type"/>.
        /// </summary>
        /// <param name="o">A type to load an attribute from.</param>
        /// <returns></returns>
        private Attribute? TryGetFrom(Type o)
        {
            try
            {
                return o.GetCustomAttribute(Type, CheckInherited);
            }
            catch (TypeLoadException ex)
            {
                Log.Debug($"Addon type \"{o.FullName}\" depends on missing type \"{ex.TypeName}\"; this is usually caused by the addon being built for an unsupported version of Volume Control!", ex);
            }
            catch (Exception ex)
            {
                Log.Debug($"Addon type \"{o.FullName}\" couldn't be loaded because of an exception!", ex);
            }
            return null;
        }
        private Attribute? TryGetFrom(MemberInfo o)
        {
            try
            {
                return o.GetCustomAttribute(Type, CheckInherited);
            }
            catch (TypeLoadException ex)
            {
                Log.Debug($"Addon member \"{o.Name}\" (Declared in type: {o.DeclaringType?.FullName}) depends on missing type \"{ex.TypeName}\"; this is usually caused by the addon being built for an unsupported version of Volume Control!", ex);
            }
            catch (Exception ex)
            {
                Log.Debug($"Addon member \"{o.Name}\" (Declared in type: {o.DeclaringType?.FullName}) couldn't be loaded because of an exception!", ex);
            }
            return null;
        }

        /// <summary>
        /// Gets all attributes from <paramref name="type"/> that match the given <paramref name="attributeType"/>.
        /// </summary>
        /// <param name="type">A type to search for attributes.</param>
        /// <param name="attributeType">An <see cref="AttributeWrapper"/> instance specifying a valid <see cref="Attribute"/>-derived type.</param>
        /// <returns>All matching attributes from <paramref name="type"/> along with member metadata if they're attributes on members.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="AttributeWrapper.Targets"/> specifies an unsupported <see cref="AttributeTargets"/> value.</exception>
        public static (MemberInfo?, Attribute?)[] GetFromType(Type type, AttributeWrapper attributeType)
        {
            List<(MemberInfo?, Attribute?)> l = new();
            switch (attributeType.Targets)
            {
            case AttributeTargets.Class:
                {
                    if (!type.IsClass) break;
                    if (attributeType.TryGetFrom(type) is Attribute attribute)
                        l.Add((null, attribute));
                    break;
                }
            case AttributeTargets.Method:
                {
                    foreach (MethodInfo mInfo in type.GetMethods())
                        if (attributeType.TryGetFrom(mInfo) is Attribute attribute)
                            l.Add((mInfo, attribute));
                    break;
                }
            case AttributeTargets.Property:
                {
                    foreach (PropertyInfo pInfo in type.GetProperties())
                        if (attributeType.TryGetFrom(pInfo) is Attribute attribute)
                            l.Add((pInfo, attribute));
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(Targets), attributeType.Targets, $"Volume Control doesn't support attribute target type '{attributeType.Targets:G}'!");
            }
            return l.ToArray();
        }
        /// <inheritdoc cref="GetFromType(Type, AttributeWrapper)"/>
        public (MemberInfo?, Attribute?)[] GetFromType(Type type) => GetFromType(type, this);
    }
}
