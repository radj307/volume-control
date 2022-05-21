using System.Runtime.CompilerServices;
using VolumeControl.Core;

namespace VolumeControl.Hotkeys.Attributes
{
    /// <summary>Marks the attached class type as a Hotkey Actions addon type.</summary>
    /// <inheritdoc cref="BaseAddonAttribute(string)"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ActionAddonAttribute : BaseAddonAttribute
    {
        /// <inheritdoc cref="ActionAddonAttribute"/>
        /// <param name="addonName">A string identifier that refers to this addon.<br/>Cannot contain punctuation, as determined by <see cref="char.IsPunctuation(char)"/>.<br/>This can safely be set to the name of your class using the nameof keyword.</param>
        public ActionAddonAttribute(string addonName) : base(addonName.RemoveIf(char.IsPunctuation)) { }
    }
}
