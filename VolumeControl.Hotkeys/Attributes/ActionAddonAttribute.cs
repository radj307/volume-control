using VolumeControl.Core;
using VolumeControl.Core.Extensions;

namespace VolumeControl.Hotkeys.Attributes
{
    /// <summary>Marks the attached class type as a Hotkey Actions addon type.</summary>
    /// <inheritdoc cref="BaseAddonAttribute(string)"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ActionAddonAttribute : BaseAddonAttribute
    {
        /// <inheritdoc cref="ActionAddonAttribute"/>
        /// <inheritdoc/>
        public ActionAddonAttribute(string addonName, CompatibleVersions compatibleVersions) : base(addonName.RemoveIf(char.IsPunctuation)) { }
        /// <inheritdoc cref="ActionAddonAttribute"/>
        /// <inheritdoc/>
        public ActionAddonAttribute(string addonName) : base(addonName.RemoveIf(char.IsPunctuation)) { }
    }
}
