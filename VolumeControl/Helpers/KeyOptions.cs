using Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Input.Enums;

namespace VolumeControl.Helpers
{
    public class KeyOptions : IEnumerable<KeyOptions.FriendlyKeyVM>
    {
        #region Constructor
        public KeyOptions()
        {
            Items = new();
            foreach (var keyValue in Enum.GetValues<EFriendlyKey>())
            {
                Items.Add(new(keyValue));
            }
        }
        #endregion Constructor

        #region Properties
        public List<FriendlyKeyVM> Items;
        #endregion Properties

        #region (class) FriendlyKeyVM
        public class FriendlyKeyVM : INotifyPropertyChanged
        {
            #region Constructor
            /// <summary>
            /// Creates a new <see cref="FriendlyKeyVM"/> instance with the specified <paramref name="key"/> &amp; <paramref name="name"/>.
            /// </summary>
            /// <param name="key">The <see cref="EFriendlyKey"/> enum value that this instance represents.</param>
            /// <param name="name">The localized name of the <paramref name="key"/>.</param>
            public FriendlyKeyVM(EFriendlyKey key)
            {
                Key = key;
                Loc.Instance.CurrentLanguageChanged += this.Instance_CurrentLanguageChanged;
            }
            #endregion Constructor

            #region Fields
            private const string EFriendlyKeyLocalizationPathRoot = "Enums.EFriendlyKey";
            #endregion Fields

            #region Properties
            /// <summary>
            /// Gets the <see cref="EFriendlyKey"/> enum that this instance represents.
            /// </summary>
            public EFriendlyKey Key { get; }
            /// <summary>
            /// Gets the localized name of this key.
            /// </summary>
            public string Name
            {
                get
                {
                    var enumName = Enum.GetName(Key)!;
                    return Loc.Tr($"{EFriendlyKeyLocalizationPathRoot}.{enumName}", defaultText: enumName);
                }
            }
            #endregion Properties

            #region Events
            public event PropertyChangedEventHandler? PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
            #endregion Events

            #region Operators
            public static implicit operator EFriendlyKey(FriendlyKeyVM inst) => inst.Key;
            #endregion Operators

            #region Methods
            public override string ToString() => Name;
            #endregion Methods

            #region EventHandlers
            private void Instance_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
                => NotifyPropertyChanged(nameof(Name));
            #endregion EventHandlers
        }
        #endregion (class) FriendlyKeyVM

        #region IEnumerable<KeyOptions.FriendlyKeyVM> Implementation
        public IEnumerator<FriendlyKeyVM> GetEnumerator() => ((IEnumerable<FriendlyKeyVM>)Items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();
        #endregion IEnumerable<KeyOptions.FriendlyKeyVM> Implementation
    }
}
