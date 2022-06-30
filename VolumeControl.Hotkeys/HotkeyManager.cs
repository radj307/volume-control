using Newtonsoft.Json;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VolumeControl.Core;
using VolumeControl.Core.Keyboard;
using VolumeControl.Hotkeys.Interfaces;
using VolumeControl.Log;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Hotkeys
{
    /// <summary>This object is responsible for managing hotkeys at runtime.</summary>
    public class HotkeyManager : INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        #region Initializers
        /// <inheritdoc cref="HotkeyManager"/>
        /// <param name="actionManager">The action manager to use.</param>
        /// <param name="loadNow">When true, the <see cref="LoadHotkeys"/> method is called from the constructor. Set this to false if you want to do it yourself.</param>
        public HotkeyManager(IHotkeyActionManager actionManager, bool loadNow = false)
        {
            _hotkeyActions = actionManager;

            if (loadNow)
                LoadHotkeys();

            this.CollectionChanged += HandleCollectionChanged;
        }
        #endregion Initializers

        #region Fields
        private readonly IHotkeyActionManager _hotkeyActions = null!;
        private bool disposedValue;
        private bool _allSelectedChanging;
        private bool? _allSelected;
        #endregion Fields

        #region Events
        /// <summary>Forwards all collection changed events from <see cref="Hotkeys"/>.</summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => ((INotifyCollectionChanged)Hotkeys).CollectionChanged += value;
            remove => ((INotifyCollectionChanged)Hotkeys).CollectionChanged -= value;
        }
        private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;
            foreach (IBindableHotkey item in e.NewItems)
            {
                item.PropertyChanged += HotkeyOnPropertyChanged;
                item.PropertyChanged += NotifyPropertyChanged;
            }
        }

        /// <summary>Triggered after the value changes when a property's setter is called.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        /// <summary>Triggers the <see cref="PropertyChanged"/> event.</summary>
        /// <param name="propertyName">This is automatically retrieved using reflection; it is the name of the property that was changed.</param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        /// <summary>
        /// Action manager object
        /// </summary>
        public IHotkeyActionManager Actions => _hotkeyActions;
        /// <summary>
        /// List of hotkeys.
        /// </summary>
        public ObservableList<IBindableHotkey> Hotkeys { get; } = new();
        /// <summary>
        /// This is used as the binding source for the (un)register all checkbox in the column header.
        /// </summary>
        public bool? AllSelected
        {
            get => _allSelected;
            set
            {
                if (value == _allSelected) return;
                _allSelected = value;

                // Set all other CheckBoxes
                AllSelectedChanged();
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Methods
        #region HotkeysListManipulators
        /// <summary>
        /// Create a new hotkey and add it to <see cref="Hotkeys"/>.
        /// </summary>
        public void AddHotkey(IBindableHotkey hk)
        {
            hk.PropertyChanged += NotifyPropertyChanged;
            Hotkeys.Add(hk);
            Log.Info($"Created a new hotkey entry:", JsonConvert.SerializeObject(hk));
            RecheckAllSelected();
        }
        /// <summary>
        /// Create a new hotkey and add it to <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="name">The name of the new hotkey.</param>
        /// <param name="key">The primary key of the hotkey.</param>
        /// <param name="modifier">The modifier keys of the hotkey.</param>
        /// <param name="actionName">The associated action of the new hotkey.</param>
        /// <param name="registerNow">When true, the new hotkey is registered immediately after construction.</param>
        public void AddHotkey(string name, Key key, Modifier modifier, string? actionName, bool registerNow = false) =>
            AddHotkey(new BindableHotkey()
            {
                Name = name,
                Key = key,
                Modifier = modifier,
                Action = actionName is null ? null : Actions[actionName],
                Registered = registerNow
            });
        /// <summary>
        /// Create a new blank hotkey and add it to <see cref="Hotkeys"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="BindableHotkey.Name"/> = <see cref="string.Empty"/>
        /// </remarks>
        public void AddHotkey() => AddHotkey(new BindableHotkey());
        /// <summary>
        /// Remove the specified hotkey from <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="hk">The <see cref="Hotkey"/> object to remove.<br/>If this is null, nothing happens.</param>
        public void DelHotkey(BindableHotkey? hk)
        {
            if (hk == null)
                return;
            Hotkeys.Remove(hk);
            Log.Info($"Deleted hotkey {hk.ID} '{hk.Name}'");
            RecheckAllSelected();
        }
        /// <summary>
        /// Remove the specified hotkey from <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="id">The ID number of the hotkey to delete.</param>
        public void DelHotkey(int id)
        {
            for (int i = Hotkeys.Count - 1; i >= 0; --i)
            {
                if (Hotkeys[i].ID.Equals(id))
                {
                    Hotkeys.RemoveAt(i);
                }
            }
            RecheckAllSelected();
        }
        /// <summary>
        /// Deletes all hotkeys in the list by first disposing them, then removing them from the list.
        /// </summary>
        public void DelAllHotkeys()
        {
            for (int i = Hotkeys.Count - 1; i >= 0; --i)
            {
                Hotkeys[i].Dispose();
                Hotkeys.RemoveAt(i);
            }
            RecheckAllSelected();
        }
        #endregion HotkeysListManipulators

        #region HotkeysListGetters
        /// <summary>
        /// Gets a single <see cref="Hotkey"/> from the list by checking for matching <paramref name="id"/> numbers.
        /// </summary>
        /// <param name="id">The ID number of the target hotkey.<br/><i>(This is compared to the <see cref="Hotkey.ID"/> property.)</i></param>
        /// <returns>The target <see cref="Hotkey"/> instance if successful; otherwise <see langword="null"/>.</returns>
        public IBindableHotkey? GetHotkey(int id) => Hotkeys.FirstOrDefault(hk => hk is not null && hk.ID.Equals(id), null);
        #endregion HotkeysListGetters

        #region HotkeysListSaveLoad
        /// <summary>
        /// Loads hotkeys from the settings file and binds them to the associated actions.
        /// </summary>
        /// <remarks><b>Make sure that the <see cref="Actions"/> property is set and initialized before calling this!</b></remarks>
        public void LoadHotkeys()
        {
            foreach (var hkjw in Settings.Hotkeys ??= Config.Hotkeys_Default)
            {
                AddHotkey(new BindableHotkey()
                {
                    Name = hkjw.Name,
                    Key = hkjw.Key,
                    Modifier = hkjw.Modifier,
                    Action = hkjw.ActionIdentifier is null ? null : Actions[hkjw.ActionIdentifier],
                    Registered = hkjw.Registered
                });
            }
            RecheckAllSelected();
        }
        /// <summary>
        /// Saves all hotkeys to the settings file.
        /// </summary>
        public void SaveHotkeys()
        {
            List<BindableHotkeyJsonWrapper> l = new();
            foreach (var hk in Hotkeys)
            {
                l.Add(new BindableHotkeyJsonWrapper()
                {
                    Key = hk.Key,
                    Modifier = hk.Modifier,
                    Registered = hk.Registered,
                    Name = hk.Name,
                    ActionIdentifier = hk.Action?.Identifier
                });
            }
            Settings.Hotkeys = l.ToArray();
            Settings.Save();
        }
        /// <summary>Resets the current hotkey list by replacing it with <see cref="Config.Hotkeys_Default"/>.</summary>
        public void ResetHotkeys()
        {
            DelAllHotkeys();
            Settings.Hotkeys = null!;
            Settings.Save();
            LoadHotkeys();
        }
        #endregion HotkeysListSaveLoad

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SaveHotkeys(); //< this saves hotkeys to the settings file
                    DelAllHotkeys(); //< this cleans up Windows API hotkey registrations
                }

                disposedValue = true;
            }
        }
        /// <summary>Finalizer</summary>
        ~HotkeyManager() { Dispose(disposing: false); }
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable

        #region AllSelected
        private void HotkeyOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            // Only re-check if the IsChecked property changed
            if (args.PropertyName == nameof(Hotkey.Registered))
                RecheckAllSelected();

            // save changes to configuration
            SaveHotkeys();
            Log.Debug($"{nameof(HotkeyManager)}:  Saved Hotkey Configuration.");
        }
        private void AllSelectedChanged()
        {
            // Has this change been caused by some other change?
            // return so we don't mess things up
            if (_allSelectedChanging) return;

            try
            {
                _allSelectedChanging = true;

                // this can of course be simplified
                if (AllSelected == true)
                {
                    foreach (IBindableHotkey hk in Hotkeys)
                        hk.Registered = true;
                }
                else if (AllSelected == false)
                {
                    foreach (IBindableHotkey hk in Hotkeys)
                        hk.Registered = false;
                }
            }
            finally
            {
                _allSelectedChanging = false;
            }
        }
        private void RecheckAllSelected()
        {
            // Has this change been caused by some other change?
            // return so we don't mess things up
            if (_allSelectedChanging) return;

            try
            {
                _allSelectedChanging = true;

                if (Hotkeys.Count > 0)
                {
                    bool prev = Hotkeys.First().Registered;
                    bool fullLoop = true;
                    for (int i = 1; i < Hotkeys.Count; ++i)
                    {
                        if (Hotkeys[i].Registered != prev)
                        {
                            fullLoop = false;
                            AllSelected = null;
                            break;
                        }
                    }
                    if (fullLoop)
                        AllSelected = prev;
                }
                else AllSelected = false;
            }
            finally
            {
                _allSelectedChanging = false;
            }
        }
        #endregion AllSelected
        #endregion Methods
    }
}
