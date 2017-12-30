using System;

namespace Toastify.Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumReadableNameAttribute : Attribute
    {
        public static readonly EnumReadableNameAttribute Default = new EnumReadableNameAttribute();

        public string Name { get; }

        public EnumReadableNameAttribute() : this(null)
        {
        }

        public EnumReadableNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}