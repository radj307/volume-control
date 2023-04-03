using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VolumeControl.Log;
using VolumeControl.SDK;

namespace VolumeControl.Helpers.Addon
{
    /// <summary>
    /// Abstract base class that represents a Volume Control addon of some type.
    /// </summary>
    public abstract class VCAddon
    {
        #region Constructor
        /// <summary>
        /// Initializes the <see cref="TargetAttributeTypes"/> property with the given <paramref name="targetAttributeTypes"/>.
        /// </summary>
        /// <param name="targetAttributeTypes">Any number of <see cref="AttributeWrapper"/> instances that specify which attributes to look for when enumerating the types in an addon assembly.</param>
        public VCAddon(params AttributeWrapper[] targetAttributeTypes)
            => TargetAttributeTypes = targetAttributeTypes.ToList();
        #endregion Constructor

        #region Fields
        /// <summary>
        /// List of <see cref="AttributeWrapper"/> instances that specify which attributes to look for when enumerating over the types in an addon assembly.
        /// </summary>
        protected readonly List<AttributeWrapper> TargetAttributeTypes;
        #endregion Fields

        #region Properties
        protected static LogWriter Log => FLog.Log;
        protected static Core.Config Settings => (Core.Config.Default as Core.Config)!;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Searches the given <paramref name="type"/>'s custom attributes for attributes with the same type as one of the <see cref="TargetAttributeTypes"/> items.
        /// </summary>
        /// <param name="type">An addon <see cref="Type"/> to search for custom attributes.</param>
        /// <returns>
        /// An array of tuples where the first item is an optional <see cref="MemberInfo"/> object representing the member that the attribute was found on, while the second item is the attribute instance that was found.
        /// </returns>
        public (MemberInfo?, Attribute)[] FindMembersWithAttributesInType(Type type)
        {
            List<(MemberInfo?, Attribute)> l = new();
            foreach (AttributeWrapper attrType in TargetAttributeTypes) //< enumerate through all known attribute types
            {
                if (attrType.GetFromType(type) is (MemberInfo?, Attribute?)[] arr) //< find applicable members within type according to this attribute type
                { // found matching attribute types:
                    foreach ((MemberInfo? mInfo, Attribute? attr) in arr) //< validate found attributes
                    {
                        if (attr is not null)
                        {
                            l.Add((mInfo, attr));
                        }
                    }
                }
            }
            return l.ToArray();
        }
        /// <summary>
        /// Processes the <paramref name="types"/> that were loaded from addon assemblies and does something with them.
        /// </summary>
        /// <param name="api">The <see cref="VCAPI"/> instance used by Volume Control.</param>
        /// <param name="types">Any number of addon types from an addon assembly.</param>
        protected abstract void HandleAddonTypes(VCAPI api, IEnumerable<Type> types);
        /// <summary>
        /// Calls <see cref="HandleAddonTypes(VCAPI, IEnumerable{Type})"/> with <see cref="VCAPI.Default"/>.
        /// </summary>
        /// <param name="types">Any number of addon types from an addon assembly.</param>
        public void LoadTypes(IEnumerable<Type> types) => HandleAddonTypes(VCAPI.Default, types);
        /// <summary>
        /// Calls <see cref="HandleAddonTypes(VCAPI, IEnumerable{Type})"/> with <see cref="VCAPI.Default"/>.
        /// </summary>
        /// <param name="types">Any number of addon types from an addon assembly.</param>
        public void LoadTypes(params Type[] types) => HandleAddonTypes(VCAPI.Default, types);
        /// <summary>
        /// Calls <see cref="LoadTypes(IEnumerable{Type})"/> with the given assembly's exported types.
        /// </summary>
        /// <param name="asm">An addon <see cref="Assembly"/> instance.</param>
        public void LoadAssembly(Assembly asm) => LoadTypes(asm.GetExportedTypes());
        /// <summary>
        /// Loads the assembly specified by <paramref name="assemblyPath"/> and calls <see cref="LoadAssembly(Assembly)"/>.
        /// </summary>
        /// <param name="assemblyPath">The location of an addon DLL assembly file.</param>
        public void LoadAssemblyFrom(string assemblyPath) => LoadAssembly(Assembly.LoadFrom(assemblyPath));
        #endregion Methods
    }
}
