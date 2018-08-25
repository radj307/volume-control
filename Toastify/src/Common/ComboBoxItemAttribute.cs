using System;

namespace Toastify.Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ComboBoxItemAttribute : Attribute
    {
        #region Static Fields and Properties

        public static readonly ComboBoxItemAttribute Default = new ComboBoxItemAttribute();

        #endregion

        #region Public Properties

        public string Content { get; }

        public string Tooltip { get; }

        #endregion

        public ComboBoxItemAttribute() : this(null)
        {
        }

        public ComboBoxItemAttribute(string content) : this(content, null)
        {
        }

        public ComboBoxItemAttribute(string content, string tooltip)
        {
            this.Content = content;
            this.Tooltip = tooltip;
        }
    }
}