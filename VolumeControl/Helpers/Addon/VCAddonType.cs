using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VolumeControl.Log;
using VolumeControl.SDK;

namespace VolumeControl.Helpers.Addon
{
    public abstract class VCAddonType
    {
        public VCAddonType(params AttributeWrapper[] attributeTypes)
        {
            _attributeTypes = attributeTypes.ToList();
        }

        protected readonly List<AttributeWrapper> _attributeTypes;

        public (MemberInfo?, Attribute)[]? FindMembersWithAttributesInType(Type t)
        {
            List<(MemberInfo?, Attribute)> l = new();
            foreach (AttributeWrapper attrType in _attributeTypes) //< enumerate through all known attribute types
            {
                if (attrType.GetFromType(t) is (MemberInfo?, Attribute?)[] arr) //< find applicable members within type according to this attribute type
                {
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

        protected static LogWriter Log => FLog.Log;
        protected static Core.Config Settings => (Core.Config.Default as Core.Config)!;

        protected abstract void HandleAddonTypes(VCAPI api, IEnumerable<Type> types);
        public void LoadTypes(IEnumerable<Type> types) => HandleAddonTypes(VCAPI.Default, types);
        public void LoadTypes(params Type[] types) => LoadTypes(types.AsEnumerable());
        public void LoadAssembly(Assembly asm) => LoadTypes(asm.GetExportedTypes());
        public void LoadAssemblyFrom(string assemblyPath) => LoadTypes(Assembly.LoadFrom(assemblyPath).GetExportedTypes());
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayTargetAttribute : ValidationAttribute
    {
        public DisplayTargetAttribute([CallerMemberName] string displayTargetName = "")
        {
            Name = displayTargetName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context) =>
                (value is not ListDisplayTarget)
                ? new ValidationResult($"{nameof(DisplayTargetAttribute)} can only be applied to properties of type \"{typeof(ListDisplayTarget).FullName}\"!")
                : ValidationResult.Success;


        /// <summary>
        /// Gets or sets the name of the display target.
        /// </summary>
        public string Name { get; set; }
        public bool InsertSpacesInName { get; set; }

        #region Methods
        /// <summary>
        /// Returns the result of replacing all occurrences of the regular expression "<see langword="\B([A-Z])"/>" with the replacement expression "<see langword=" $1"/>", effectively inserting a space between all capitalized words in the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string InsertSpacesIn(string str) => Regex.Replace(str, "\\B([A-Z])", " $1");
        public string GetNameString() => InsertSpacesInName ? InsertSpacesIn(Name) : Name;
        #endregion Methods
    }

    /*public class VCDisplayTargetAddon : VCAddonType
    {
        public VCDisplayTargetAddon() : base(new AttributeWrapper(typeof(DisplayTargetAttribute)))
        { }

        protected override void HandleAddonTypes(VCAPI api, IEnumerable<ValueType> types)
        {
            List<ListDisplayTarget> displayTargets = new();


        }
    }*/
}
