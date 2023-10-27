using System.ComponentModel;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Provides data for the hotkey Registering and Unregistering events.
    /// </summary>
    public sealed class HotkeyRegisteringEventArgs : HandledEventArgs
    {
        #region Constructors
        internal HotkeyRegisteringEventArgs(bool initialState, bool registrationStateWhenHandled) : base(initialState)
        {
            RegistrationSuccessStateWhenHandled = registrationStateWhenHandled;
        }
        internal HotkeyRegisteringEventArgs(bool initialState) : base(initialState) { }
        internal HotkeyRegisteringEventArgs() { }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets whether the (un)registration succeeded when Handled is set to <see langword="true"/>.
        /// </summary>
        /// <returns><see langword="true"/> for success; <see langword="false"/> for failure.</returns>
        public bool RegistrationSuccessStateWhenHandled { get; set; }
        #endregion Properties
    }
    /// <summary>
    /// Represents a method that can handle Registering and Unregistering hotkey events.
    /// </summary>
    /// <param name="sender">The hotkey that is trying to (un)register.</param>
    /// <param name="e">The <see cref="HotkeyRegisteringEventArgs"/> instance for this event invocation.</param>
    public delegate void HotkeyRegisteringEventHandler(object sender, HotkeyRegisteringEventArgs e);
}
