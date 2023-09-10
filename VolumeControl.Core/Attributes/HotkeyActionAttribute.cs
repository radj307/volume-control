using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Media;
using VolumeControl.Core.Input.Actions;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Indicates that a method defines a hotkey action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HotkeyActionAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Indicates that the attached method defines a hotkey action.
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
        /// Gets or sets the name of this hotkey action.
        /// </summary>
        /// <remarks>
        /// When <see cref="InsertSpacesInName"/> is <see langword="true"/>, the name is modified before being used.
        /// </remarks>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description of this hotkey action.
        /// </summary>
        /// <remarks>
        /// This is displayed as a tooltip in the list of hotkey actions.
        /// </remarks>
        public string? Description { get; set; } = null;
        /// <summary>
        /// Gets or sets the sorting group of this hotkey action.
        /// Actions with the same group name appear next to each other in the list of hotkey actions.
        /// </summary>
        /// <remarks>
        /// This overrides <see cref="HotkeyActionGroupAttribute.GroupName"/> for this action.
        /// </remarks>
        public string? GroupName { get; set; } = null;
        /// <summary>
        /// Gets or sets the color of the <see cref="GroupName"/> string when it is displayed.
        /// </summary>
        /// <remarks>
        /// This overrides <see cref="HotkeyActionGroupAttribute.GroupColor"/> for this action.
        /// </remarks>
        public string? GroupColor { get; set; }
        /// <summary>
        /// Gets or sets whether the <see cref="Name"/> property will be formatted before being used.
        /// </summary>
        /// <remarks>
        /// When set to <see langword="true"/>, the <see cref="Name"/> property is passed through the <see cref="SeparateWordsInString(string)"/> function to insert spaces between capitalized letters;
        /// otherwise when <see langword="false"/>, the <see cref="Name"/> property is not modified and is used as-is.
        /// </remarks>
        public bool InsertSpacesInName { get; set; } = true;
        #endregion Properties

        #region SeparateWordsInString
        /// <summary>
        /// Separates <paramref name="str"/> by inserting a space before each capitalized letter that appears directly after at least one non-whitespace character.
        /// </summary>
        /// <remarks>
        /// This works by replacing occurrences of the regular expression "<see langword="\B([A-Z])"/>" with the replacement expression "<see langword=" $1"/>".
        /// </remarks>
        /// <param name="str">A <see cref="string"/> to separate.</param>
        /// <returns>The given string with a single space character inserted between each word.</returns>
        public static string SeparateWordsInString(string str) => Regex.Replace(str, "\\B([A-Z])", " $1");
        #endregion SeparateWordsInString

        #region Methods
        #region Private
        private string GetNameString() => this.InsertSpacesInName ? SeparateWordsInString(this.Name) : this.Name;
        private Brush? GetGroupBrush() => this.GroupColor is null ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.GroupColor));
        #endregion Private

        #region Public
        /// <summary>
        /// Gets a new <see cref="HotkeyActionData"/> struct that contains all of the data for this hotkey action.
        /// </summary>
        /// <returns>A new <see cref="HotkeyActionData"/> struct.</returns>
        public HotkeyActionData GetActionData()
            => new(this.GetNameString(), this.Description, this.GroupName, this.GetGroupBrush());
        #endregion Public
        #endregion Methods
    }
}
