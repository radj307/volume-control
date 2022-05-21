using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace VolumeControl.Hotkeys.Attributes
{
    /// <summary>Indicates that a method is a valid hotkey action.<br/>If you don't specify a name parameter, it is parsed from the name of the method it is attached to.</summary>
    /// <remarks>To create a hotkey action, attach this attribute to the owning method.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class HotkeyActionAttribute : Attribute
    {
        #region Constructor
        /// <inheritdoc cref="HotkeyActionAttribute"/>
        /// <param name="actionName">The name of the action being called.</param>
        public HotkeyActionAttribute([CallerMemberName] string actionName = "") => _originalName = actionName;
        #endregion Constructor

        #region Fields
        private readonly string _originalName;
        private string? _actionName = null;
        private bool _interpolateName = true;
        #endregion Fields

        #region Properties
        /// <summary>The name shown in the GUI when selecting a hotkey action.<br/>This cannot be changed directly post-construction time, but may be influenced by <see cref="InterpolateName"/> after construction.<br/>If this is set to an empty string, the action is always ignored and will never be selectable.</summary>
        /// <remarks><b>Note that this is also the name used when saving hotkeys to the config file.</b></remarks>
        public string ActionName => _actionName ??= (InterpolateName ? ParseActionName(_originalName) : _originalName);
        /// <summary>
        /// Gets or sets whether the action name is interpolated by inserting spaces between each of the uppercase letters to split the name by words.<br/>
        /// If your action is named using abbreviations or you don't want this behaviour to occur, set this to false.
        /// </summary>
        /// <remarks><b>Note that when this property changes, the <see cref="ActionName"/> property is invalidated causing it to be regenerated the next time it is requested.</b></remarks>
        public bool InterpolateName
        {
            get => _interpolateName;
            set
            {
                if (_interpolateName != value)
                {
                    _interpolateName = value;
                    _actionName = null;
                }
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>Parses the given string using regular expressions to insert spaces between words in a method name.</summary>
        /// <param name="name">Input string.</param>
        /// <returns><paramref name="name"/> where each camel cased word is separated by a space character.</returns>
        public static string ParseActionName(string name) => Regex.Replace(name, "\\B([A-Z])", " $1");
        #endregion Methods
    }
}
