namespace VolumeControl.Core.HotkeyActions.Attributes
{
    /// <summary>
    /// This attribute is used to indicate that a method is a hotkey action binding target.
    /// </summary>
    /// <remarks>
    /// If you don't specify any parameters, the name of the method is used as the action name.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HandlerAttribute : Attribute
    {
        public HandlerAttribute() { }
        public HandlerAttribute(string actionName)
        {
            ActionName = actionName;
        }

        /// <summary>
        /// This overrides the default action name shown in the action dropdown.<br/>
        /// If this is set to null, the method name is used by default.
        /// </summary>
        public string? ActionName { get; } = null;
    }
}
