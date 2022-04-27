using VolumeControl.Core.Enum;

namespace VolumeControl.Core.Events
{
    public class HotkeyPressedEventArgs : EventArgs
    {
        public HotkeyPressedEventArgs(VolumeControlSubject subject = VolumeControlSubject.NULL, VolumeControlAction action = VolumeControlAction.NULL, object? data = null)
        {
            Subject = subject;
            Action = action;
            Data = data;
        }

        /// <summary>
        /// The subject of the triggered hotkey.
        /// </summary>
        public readonly VolumeControlSubject Subject;
        /// <summary>
        /// The action of the triggered hotkey.
        /// </summary>
        public readonly VolumeControlAction Action;
        /// <summary>
        /// This is an optional property that contains different values depending on the subject and action.
        /// </summary>
        /// <remarks>
        /// This is a list of potential values for each type:
        /// <list type="table">
        ///     <item>
        ///         <term><see cref="VolumeControlSubject.VOLUME"/></term><description>Potential value types for the volume subject:<list type="table">
        ///             <item><term><see cref="VolumeControlAction.INCREMENT"/></term><description>(<see cref="decimal"/>) The currently selected process' volume level after incrementing.</description></item>
        ///             <item><term><see cref="VolumeControlAction.DECREMENT"/></term><description>(<see cref="decimal"/>) The currently selected process' volume level after decrementing.</description></item>
        ///             <item><term><see cref="VolumeControlAction.TOGGLE"/></term><description>(<see cref="bool"/>) The currently selected process' mute state.</description></item>
        ///         </list></description>
        ///     </item>
        ///     <item><term><see cref="VolumeControlSubject.MEDIA"/></term><description>(<see cref="null"/>) Media subjects do not use this property.</description></item>
        ///     <item>
        ///         <term><see cref="VolumeControlSubject.TARGET"/></term><description><list type="table">
        ///             <item><term><see cref="VolumeControlAction.INCREMENT"/></term><description>(<see cref="Audio.IAudioProcess?"/>) The currently selected process.</description></item>
        ///             <item><term><see cref="VolumeControlAction.DECREMENT"/></term><description>(<see cref="Audio.IAudioProcess?"/>) The currently selected process.</description></item>
        ///             <item><term><see cref="VolumeControlAction.TOGGLE"/></term><description>(<see cref="bool"/>) The current target lock state.</description></item>
        ///         </list></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public readonly object? Data;
    }
    public delegate void HotkeyPressedEventHandler(object? sender, HotkeyPressedEventArgs e);
}
