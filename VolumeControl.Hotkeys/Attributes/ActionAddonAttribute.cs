namespace VolumeControl.Hotkeys.Attributes
{
    /// <summary>
    /// Specifies that an object is a Hotkey action addon.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ActionAddonAttribute : Attribute { }
}
