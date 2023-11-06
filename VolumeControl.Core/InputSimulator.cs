using InputSimulatorEx;
using InputSimulatorEx.Native;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Extensions;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Core
{
    /// <summary>
    /// Provides a convenient way to synthesize keystrokes and other input.<br/>
    /// See <see cref="InputSimulatorEx.InputSimulator"/>.
    /// </summary>
    public static class InputSimulator
    {
        #region Fields
        private static readonly InputSimulatorEx.InputSimulator _simulator = new();
        #endregion Fields

        #region Properties
        /// <summary>
        /// Synthesizes keyboard events.
        /// </summary>
        public static IKeyboardSimulator Keyboard => _simulator.Keyboard;
        /// <summary>
        /// Synthesizes mouse events.
        /// </summary>
        public static IMouseSimulator Mouse => _simulator.Mouse;
        /// <summary>
        /// Input device state manager.
        /// </summary>
        public static IInputDeviceStateAdaptor InputDeviceState => _simulator.InputDeviceState;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Synthesizes a key press event for the given key.<br/>
        /// Also accepts any number of modifier keys.
        /// </summary>
        /// <param name="vk">The <see cref="VirtualKeyCode"/> of the key to simulate.</param>
        /// <param name="modifiers">Any number of <see cref="VirtualKeyCode"/> modifier keys.</param>
        public static void SendKey(VirtualKeyCode vk, params VirtualKeyCode[] modifiers) => Keyboard.ModifiedKeyStroke(modifiers, vk);
        /// <param name="vk">The <see cref="EVirtualKeyCode"/> of the key to simulate.</param>
        /// <param name="modifiers">Any number of <see cref="EVirtualKeyCode"/> modifier keys.</param>
        /// <inheritdoc cref="SendKey(VirtualKeyCode, VirtualKeyCode[])"/>
        public static void SendKey(EVirtualKeyCode vk, params EVirtualKeyCode[] modifiers) => SendKey(vk.GetVirtualKeyCodeEx(), modifiers.ConvertEach(evk => evk.GetVirtualKeyCodeEx()).ToArray());
        /// <inheritdoc cref="SendKey(VirtualKeyCode, VirtualKeyCode[])"/>
        /// <param name="key">The key to simulate being pressed.</param>
        /// <param name="modifierKeys">The modifier keys to simulate being pressed.</param>
        public static void SendKey(System.Windows.Input.Key key, System.Windows.Input.ModifierKeys modifierKeys)
            => Keyboard.ModifiedKeyStroke(
                modifierKeys.GetSingleValues().Select(mod => (VirtualKeyCode)System.Windows.Input.KeyInterop.VirtualKeyFromKey(mod.ToKey())),
                (VirtualKeyCode)System.Windows.Input.KeyInterop.VirtualKeyFromKey(key));
        /// <summary>
        /// Synthesizes key presses to 'type' the given <see cref="char"/>.
        /// </summary>
        /// <param name="c">A <see cref="char"/> associated with a keyboard key.</param>
        public static void SendText(char c) => Keyboard.TextEntry(c);
        /// <summary>
        /// Synthesizes key presses to 'type' the given <see cref="string"/>.
        /// </summary>
        /// <param name="s">A <see cref="string"/> of characters associated with keyboard keys.</param>
        public static void SendText(string s) => Keyboard.TextEntry(s);
        #endregion Methods
    }
}
