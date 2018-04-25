using System;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ToastifyAPI.Model.Interfaces;
using MouseAction = ToastifyAPI.Core.MouseAction;

namespace ToastifyAPI.Model
{
    /// <summary>
    /// Represents either a <see cref="T:System.Windows.Input.Key"/> or a <see cref="MouseAction"/>.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public sealed class KeyOrButton : IKeyOrButton, IEquatable<KeyOrButton>
    {
        public bool IsKey
        {
            get { return this.Key.HasValue; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public Key? Key { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MouseAction? MouseButton { get; }

        public KeyOrButton(Key key)
        {
            this.Key = key;
        }

        public KeyOrButton(MouseAction mouseAction)
        {
            this.MouseButton = mouseAction;
        }

        [JsonConstructor]
        public KeyOrButton(bool isKey, Key? key, MouseAction? mouseButton)
        {
            if (isKey && key.HasValue)
                this.Key = key;
            else if (!isKey && mouseButton.HasValue)
                this.MouseButton = mouseButton;
            else
                throw new ArgumentException($"Invalid arguments ({isKey}, {key}, {mouseButton})");
        }

        /// <inheritdoc />
        public object Clone()
        {
            // ReSharper disable once PossibleInvalidOperationException
            return this.IsKey
                ? new KeyOrButton(this.Key.Value)
                : this.MouseButton.HasValue ? new KeyOrButton(this.MouseButton.Value) : throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public bool Equals(KeyOrButton other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return this.Key == other.Key &&
                   this.MouseButton == other.MouseButton;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == this.GetType() && this.Equals((KeyOrButton)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Key.GetHashCode() * 397) ^ this.MouseButton.GetHashCode();
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            // ReSharper disable once PossibleInvalidOperationException
            return this.IsKey
                ? $"{this.Key.Value}"
                : this.MouseButton.HasValue ? $"{this.MouseButton.Value}" : string.Empty;
        }

        public static implicit operator KeyOrButton(Key key)
        {
            return new KeyOrButton(key);
        }

        public static implicit operator KeyOrButton(MouseAction mouseAction)
        {
            return new KeyOrButton(mouseAction);
        }
    }
}