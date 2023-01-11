using System.ComponentModel;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Event arguments for the <see cref="IHotkeyAction.HandleKeyEvent(object?, HotkeyActionPressedEventArgs)"/> handler method.
    /// </summary>
    public class HotkeyActionPressedEventArgs : HandledEventArgs
    {
        #region Constructor
        /// <summary>
        /// Creates a new default <see cref="HotkeyActionPressedEventArgs"/> instance.
        /// </summary>
        public HotkeyActionPressedEventArgs() { }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionPressedEventArgs"/> instance.
        /// </summary>
        /// <param name="actionSettings">Any action settings required to call the action method.</param>
        public HotkeyActionPressedEventArgs(IList<HotkeyActionSetting>? actionSettings) => ActionSettings = actionSettings;
        /// <summary>
        /// Creates a new <see cref="HotkeyActionPressedEventArgs"/> instance.
        /// </summary>
        /// <param name="actionSettings">Any action settings required to call the action method.</param>
        /// <param name="defaultHandledValue">Default value for the <see cref="HandledEventArgs.Handled"/> property.</param>
        public HotkeyActionPressedEventArgs(IList<HotkeyActionSetting>? actionSettings, bool defaultHandledValue) : base(defaultHandledValue) => ActionSettings = actionSettings;
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Contains any action settings that are required to call the action method that this object is passed to.
        /// </summary>
        public IList<HotkeyActionSetting>? ActionSettings { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the action setting specified by <paramref name="name"/> from <see cref="ActionSettings"/>.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use for string comparisons.</param>
        /// <returns>The matching <see cref="HotkeyActionSetting"/> if found; otherwise <see langword="null"/>.</returns>
        public HotkeyActionSetting? FindActionSetting(string name, StringComparison stringComparison = StringComparison.Ordinal) => ActionSettings?.FirstOrDefault(item => item.Label.Equals(name, stringComparison));
        #endregion Methods
    }
    /// <inheritdoc cref="IHotkeyAction.HandleKeyEvent(object?, HotkeyActionPressedEventArgs)"/>
    public delegate void HotkeyActionPressedEventHandler(object? sender, HotkeyActionPressedEventArgs e);
}
