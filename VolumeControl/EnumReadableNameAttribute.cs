using System;

namespace VolumeControl
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumReadableNameAttribute : Attribute
    {
        #region Static Fields and Properties

        public static readonly EnumReadableNameAttribute Default = new EnumReadableNameAttribute();

        #endregion

        #region Public Properties

        public string Name { get; }

        #endregion

        public EnumReadableNameAttribute() : this(null)
        {
        }

        public EnumReadableNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}