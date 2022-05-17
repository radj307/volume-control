using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace VolumeControl.Core.HotkeyActions.Attributes
{
    /// <summary>Indicates that a method is a valid hotkey action.<br/>If you don't specify parameters, the name is parsed from the name of the method it is attached to.</summary>
    /// <remarks>This is the only requirement for creating hotkey actions, other than passing the containing object to the <see cref="ActionBindings"/> interface.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HotkeyActionAttribute : Attribute
    {
        /// <inheritdoc cref="HotkeyActionAttribute"/>
        /// <param name="splitActionNameCamelCase">When true, <paramref name="myName"/> is parsed using <see cref="ParseActionName(string)"/></param>
        /// <param name="myName">The name of the action being called.</param>
        public HotkeyActionAttribute(bool splitActionNameCamelCase, [CallerMemberName] string myName = "")
        {
            if (splitActionNameCamelCase)
                ActionName = ParseActionName(myName);
            else
                ActionName = myName;
        }
        /// <inheritdoc cref="HotkeyActionAttribute(bool, string)"/>
        public HotkeyActionAttribute([CallerMemberName] string myName = "") : this(!myName.Contains(' '), myName) { }

        /// <summary>
        /// This overrides the default action name shown in the action dropdown.<br/>
        /// If this is set to null, the method name is used by default.
        /// </summary>
        public string ActionName { get; }

        /// <summary>Parses the given string using regular expressions to insert spaces between words in a method name.</summary>
        /// <param name="name">Input string.</param>
        /// <returns><paramref name="name"/> where each camel cased word is separated by a space character.</returns>
        public static string ParseActionName(string name) => Regex.Replace(name, "\\B([A-Z])", " $1");
    }
}
