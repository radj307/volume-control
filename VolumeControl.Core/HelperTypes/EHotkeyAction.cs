namespace VolumeControl.Core.HelperTypes
{
    /// <summary>
    /// This enumerator defines the various types of actions that hotkeys may be configured to perform.
    /// </summary>
    /// <remarks>
    /// To add additional hotkey actions, follow this process:<br/>
    /// <list type="number" start="1">
    /// <item><description>Add a new value to the <see cref="EHotkeyAction"/> enumerator with a descriptive name.</description></item>
    /// <item><description>Create a <see cref="HotkeyLib.KeyEventHandler"/> that will be called whenever a bound hotkey is pressed.<br/>Most of the current handlers are located within <see cref="AudioAPI"/> as they pertain to audio controls.<br/>The method must have a signature similar to the following:
    /// <code language="cs">
    /// void MyHandlerName(<see cref="object?"/> sender, <see cref="System.ComponentModel.HandledEventArgs"/> e){}
    /// </code>
    /// </description></item>
    /// <item><description>Add a new entry to <see cref="HotkeyActionBindings.Bindings"/>.<br/>The key is the <see cref="EHotkeyAction"/> enum you added in step 1.<br/>The value is the handler you created in step 2.</description></item>
    /// </list>
    /// </remarks>
    public enum EHotkeyAction : byte
    {
        None,
        VolumeUp,
        VolumeDown,
        ToggleMute,
        NextTrack,
        PreviousTrack,
        TogglePlayback,
        NextTarget,
        PreviousTarget,
        ToggleTargetLock,
        NextDevice,
        PreviousDevice,
        ToggleDeviceLock,
    }
}
