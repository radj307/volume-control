using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Media;
using VolumeControl.Core.Input.Actions;

namespace VolumeControl.Hotkeys.Attributes
{
    /// <summary>Indicates that a method is a valid hotkey action.<br/>If you don't specify a name parameter, it is parsed from the name of the method it is attached to.</summary>
    /// <remarks>To create a hotkey action, attach this attribute to the owning method.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HotkeyActionAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Use this attribute to mark the associated method as a hotkey action.
        /// </summary>
        /// <param name="name">
        /// This determines the value of the <see cref="Name"/> property, which is used in conjunction with the <see cref="InsertSpacesInName"/> &amp; <see cref="GroupName"/> properties to determine the action name shown in the GUI.<br/><br/>
        /// When this parameter is omitted, the action name is automatically set to the method name using reflection.<br/>
        /// <i>Example:  Consider a method named "SessionVolumeUp", for which the default value of <paramref name="name"/> is either "SessionVolumeUp" when <see cref="InsertSpacesInName"/> is <see langword="false"/> or "Session Volume Up" when <see cref="InsertSpacesInName"/> is <see langword="true"/>.</i>
        /// </param>
        public HotkeyActionAttribute([CallerMemberName] string name = "") => this.Name = name;
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Mandatory name to use when referencing this action.<br/>
        /// Note that when loading hotkey addons, the result of the <see cref="GetActionData"/> method is used to determine the final <see cref="HotkeyActionData.ActionName"/> string.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// An optional paragraph to display as a tooltip next to the hotkey action in the dropdown menu when the user hovers their mouse over it.<br/>
        /// When this is set to <see langword="null"/>, there will not be a tooltip when the user hovers over your action in the dropdown menu.
        /// </summary>
        public string? Description { get; set; } = null;
        /// <summary>
        /// An optional name to include as a prefix in the GUI to assist with sorting elements.<br/>
        /// Note that a seperator string is automatically appended to this, and all leading/trailing whitespace is trimmed.
        /// </summary>
        /// <remarks><b>When this is set to <see langword="null"/>, the seperator string is not included</b> in the result of the <see cref="GetActionData"/> method.</remarks>
        public string? GroupName { get; set; } = null;
        /// <summary>
        /// The color to use when painting the text of <see cref="GroupName"/> if it isn't set to <see langword="null"/>.<br/>
        /// This is converted to a <see cref="SolidColorBrush"/> using the specified color.
        /// </summary>
        public string? GroupColor { get; set; }
        /// <summary>
        /// Gets or sets a boolean that determines whether the <see cref="Name"/> property is 
        /// </summary>
        public bool InsertSpacesInName { get; set; } = true;
        #endregion Properties

        #region Methods
        #region Static
        /// <summary>
        /// Returns the result of replacing all occurrences of the regular expression "<see langword="\B([A-Z])"/>" with the replacement expression "<see langword=" $1"/>", effectively inserting a space between all capitalized words in the string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string InsertSpacesIn(string str) => Regex.Replace(str, "\\B([A-Z])", " $1");
        #endregion Static

        #region Private
        private string GetNameString() => this.InsertSpacesInName ? InsertSpacesIn(this.Name) : this.Name;
        #endregion Private

        #region Public
        /// <summary>
        /// Creates a new <see cref="HotkeyActionData"/> instance representing the final, interpolated 
        /// </summary>
        /// <returns>A new <see cref="HotkeyActionData"/> instance representing this hotkey action.</returns>
        public HotkeyActionData GetActionData() => new(this.GetNameString(), this.Description, this.GroupName, this.GroupColor is null ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.GroupColor)));
        #endregion Public
        #endregion Methods
    }
}
