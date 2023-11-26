using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace VolumeControl.WPF.CustomMessageBox
{
    /// <summary>
    /// A button for use in a <see cref="CustomMessageBox"/>.
    /// </summary>
    /// <remarks>
    /// Can be implicitly converted to <see cref="string"/> and <see cref="MessageBoxResult"/>.
    /// </remarks>
    [DebuggerDisplay("Name = {Name}, Content = {Content}")]
    public sealed class CustomMessageBoxButton : IEquatable<CustomMessageBoxButton>, IEquatable<string>, IEquatable<char>, IEquatable<MessageBoxResult>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxButton"/> instance with the specified <paramref name="name"/> and <paramref name="content"/>.
        /// </summary>
        /// <param name="name">The name of this button.</param>
        /// <param name="content">The content to display in this button.</param>
        public CustomMessageBoxButton(string name, object content)
        {
            Name = name;
            Content = content;
        }
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxButton"/> instance with the specified <paramref name="name"/>. The <paramref name="name"/> is also used as the button's Content.
        /// </summary>
        /// <param name="name">The name of this button.</param>
        public CustomMessageBoxButton(string name)
        {
            Name = name;
            Content = name;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the unique name of this button.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the content displayed in this button.
        /// </summary>
        /// <remarks>
        /// When no content was specified in the constructor, the Name <see cref="string"/> is used instead.
        /// </remarks>
        public object Content { get; }
        #endregion Properties

        #region Operators
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxButton"/> instance from the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the button instance to create.</param>
        public static implicit operator CustomMessageBoxButton(string? name) => name != null ? new(name, name) : null!;
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxButton"/> instance from the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the button instance to create.</param>
        public static implicit operator CustomMessageBoxButton(char? name) => name.HasValue ? new(new string(name.Value, 1), name) : null!;
        /// <summary>
        /// Creates a new <see cref="CustomMessageBoxButton"/> instance from the specified <paramref name="messageBoxResult"/>.
        /// </summary>
        /// <param name="messageBoxResult">A <see cref="MessageBoxButton"/> enum value to convert.</param>
        /// <exception cref="InvalidEnumArgumentException">The specified <paramref name="messageBoxResult"/> is invalid for the <see cref="MessageBoxResult"/> enum type.</exception>
        public static explicit operator CustomMessageBoxButton(MessageBoxResult messageBoxResult) => new(Enum.GetName(messageBoxResult) ?? throw new InvalidEnumArgumentException(nameof(messageBoxResult), (int)messageBoxResult, typeof(MessageBoxResult)));
        /// <summary>
        /// Converts the specified <paramref name="cmbButton"/> to a <see cref="MessageBoxResult"/> by converting the value of <see cref="Name"/>.
        /// </summary>
        /// <param name="cmbButton">A <see cref="CustomMessageBoxButton"/> instance.</param>
        public static implicit operator MessageBoxResult(CustomMessageBoxButton? cmbButton) => Enum.TryParse(typeof(MessageBoxResult), cmbButton?.Name, out var value) ? (MessageBoxResult)value! : MessageBoxResult.None;
        /// <summary>
        /// Converts the specified <paramref name="cmbButton"/> to a <see cref="string"/> by returning the value of <see cref="Name"/>.
        /// </summary>
        /// <param name="cmbButton">A <see cref="CustomMessageBoxButton"/> instance.</param>
        public static explicit operator string?(CustomMessageBoxButton? cmbButton) => cmbButton?.Name;
        /// <summary>
        /// Checks if the Name of the specified <paramref name="left"/> instance matches the name of the <paramref name="right"/> instance.
        /// </summary>
        /// <returns><see langword="true"/> when the names match; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(CustomMessageBoxButton? left, CustomMessageBoxButton? right)
            => (left is null && right is null) || (left?.Equals(right) ?? false);
        /// <summary>
        /// Checks if the Name of the specified <paramref name="left"/> instance doesn't match the name of the <paramref name="right"/> instance.
        /// </summary>
        /// <returns><see langword="true"/> when the names don't match; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(CustomMessageBoxButton? left, CustomMessageBoxButton? right)
            => !(left is null && right is null) && !(left?.Equals(right) ?? false);
        /// <summary>
        /// Checks if the Name of the specified <paramref name="inst"/> is a match for the specified <paramref name="messageBoxResult"/>.
        /// </summary>
        /// <returns><see langword="true"/> when the name matches; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(CustomMessageBoxButton? inst, MessageBoxResult messageBoxResult)
            => inst is not null && inst.Equals(messageBoxResult);
        /// <summary>
        /// Checks if the Name of the specified <paramref name="inst"/> is not a match for the specified <paramref name="messageBoxResult"/>.
        /// </summary>
        /// <returns><see langword="true"/> when the name doesn't match; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(CustomMessageBoxButton? inst, MessageBoxResult messageBoxResult)
            => inst is not null && inst.Equals(messageBoxResult);
        /// <summary>
        /// Checks if the Name of the specified <paramref name="inst"/> is a match for the specified <paramref name="name"/>.
        /// </summary>
        /// <returns><see langword="true"/> when the names match; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(CustomMessageBoxButton? inst, string? name)
            => inst is not null && inst.Equals(name);
        /// <summary>
        /// Checks if the Name of the specified <paramref name="inst"/> is not a match for the specified <paramref name="name"/>.
        /// </summary>
        /// <returns><see langword="true"/> when the names don't match; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(CustomMessageBoxButton? inst, string? name)
            => inst is null ? name is null : !inst.Equals(name);
        /// <summary>
        /// Checks if the Name of the specified <paramref name="inst"/> is a match for the specified <paramref name="name"/>.
        /// </summary>
        /// <returns><see langword="true"/> when the names match; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(CustomMessageBoxButton? inst, char name)
            => inst is not null && inst.Equals(name);
        /// <summary>
        /// Checks if the Name of the specified <paramref name="inst"/> is not a match for the specified <paramref name="name"/>.
        /// </summary>
        /// <returns><see langword="true"/> when the names don't match; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(CustomMessageBoxButton? inst, char name)
            => inst is not null && !inst.Equals(name);
        #endregion Operators

        #region Methods
        /// <returns><see cref="Name"/></returns>
        /// <inheritdoc/>
        public override string ToString() => Name;
        /// <summary>
        /// Converts this <see cref="CustomMessageBoxButton"/> instance to a <see cref="MessageBoxResult"/> value.
        /// </summary>
        /// <returns>The <see cref="MessageBoxResult"/> value with a matching name.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Name"/> is not valid for <see cref="MessageBoxResult"/></exception>
        public MessageBoxResult ToMessageBoxResult(bool ignoreCase = true)
        {
            try
            {
                return Enum.Parse<MessageBoxResult>(Name, ignoreCase);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot convert {nameof(CustomMessageBoxButton)}; {nameof(Name)} \"{Name}\" is not valid for {nameof(MessageBoxResult)}", ex);
            }
        }
        /// <summary>
        /// Attempts to convert this <see cref="CustomMessageBoxButton"/> instance to a <see cref="MessageBoxResult"/> value.
        /// </summary>
        /// <param name="ignoreCase">When <see langword="true"/>, letter casing is ignored.</param>
        /// <param name="result">The <see cref="MessageBoxResult"/> value with a matching name.</param>
        /// <returns><see langword="true"/> when successful; otherwise, <see langword="false"/>.</returns>
        public bool TryGetMessageBoxResult(bool ignoreCase, out MessageBoxResult result)
            => Enum.TryParse(Name, ignoreCase, out result);
        /// <inheritdoc cref="TryGetMessageBoxResult(bool, out MessageBoxResult)"/>
        public bool TryGetMessageBoxResult(out MessageBoxResult result)
            => TryGetMessageBoxResult(true, out result);
        /// <summary>
        /// Checks if this instance's Name is a match for the specified <paramref name="other"/> instance's name using the specified <paramref name="stringComparison"/> type.
        /// </summary>
        /// <param name="other">Another <see cref="CustomMessageBoxButton"/> instance.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use for comparing names.</param>
        /// <returns><see langword="true"/> when both names match &amp; <paramref name="other"/> isn't <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomMessageBoxButton? other, StringComparison stringComparison) => other != null && Name.Equals(other.Name, stringComparison);
        /// <summary>
        /// Checks if this instance's Name is a match for the specified <paramref name="other"/> instance's name using <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        /// <param name="other">Another <see cref="CustomMessageBoxButton"/> instance.</param>
        /// <returns><see langword="true"/> when both names match &amp; <paramref name="other"/> isn't <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomMessageBoxButton? other) => other != null && Equals(other, StringComparison.Ordinal);
        /// <summary>
        /// Checks if this instance's Name matches the specified <paramref name="name"/> name using the specified <paramref name="stringComparison"/> type.
        /// </summary>
        /// <param name="name">A <see cref="string"/> to compare with this instance's Name.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> type to use.</param>
        /// <returns><see langword="true"/> when both names match &amp; <paramref name="name"/> isn't <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(string? name, StringComparison stringComparison) => name != null && this.Name.Equals(name, stringComparison);
        /// <summary>
        /// Checks if this instance's Name matches the specified <paramref name="name"/> name using <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        /// <param name="name">A <see cref="string"/> to compare with this instance's Name.</param>
        /// <returns><see langword="true"/> when both names match &amp; <paramref name="name"/> isn't <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(string? name) => name != null && Equals(name, StringComparison.Ordinal);
        /// <summary>
        /// Checks if this instance's Name is a match for the specified <paramref name="name"/> name.
        /// </summary>
        /// <param name="name">A <see cref="string"/> to compare with this instance's Name.</param>
        /// <returns><see langword="true"/> when Name is 1 char long and that char is equal to <paramref name="name"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(char name) => Name.Length == 1 && name.Equals(Name[0]);
        /// <summary>
        /// Checks if this instance's Name is a match for the name of the specified <paramref name="result"/>.
        /// </summary>
        /// <param name="result">A <see cref="MessageBoxResult"/> to compare with this instance's Name.</param>
        /// <returns><see langword="true"/> when Name matches the name of the <paramref name="result"/> enum value; otherwise, <see langword="false"/>.</returns>
        public bool Equals(MessageBoxResult result) => Name.Equals(Enum.GetName(result), StringComparison.Ordinal);
        /// <summary>Determines whether the specified object is equatable with, and equal to, the current object.</summary>
        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj != null && Equals(obj as CustomMessageBoxButton);
        /// <returns>A hash code for the current object's Name.</returns>
        /// <inheritdoc/>
        public override int GetHashCode() => Name.GetHashCode();
        #endregion Methods
    }
}
