using Newtonsoft.Json;
using System.Collections.Specialized;
using System.ComponentModel;
using VolumeControl.Core.Input;
using VolumeControl.Core.Interfaces;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core
{
    /// <summary>This object is responsible for managing hotkeys at runtime.</summary>
    public class HotkeyManager : INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        #region Initializers
        /// <inheritdoc cref="HotkeyManager"/>
        /// <param name="actionManager">The action manager to use.</param>
        public HotkeyManager(IHotkeyActionManager actionManager)
        {
            this.Actions = actionManager;

            CollectionChanged += this.HandleCollectionChanged;
            this.RecheckAllSelected();
        }
        #endregion Initializers

        #region Fields
        private bool disposedValue;
        private bool _allSelectedChanging;
        private bool? _allSelected;
        #endregion Fields

        #region Events
        /// <summary>Forwards all collection changed events from <see cref="Hotkeys"/>.</summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => ((INotifyCollectionChanged)this.Hotkeys).CollectionChanged += value;
            remove => ((INotifyCollectionChanged)this.Hotkeys).CollectionChanged -= value;
        }
        private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Replace:
                _ = (e.NewItems?.ForEach(i =>
                {
                    if (i is IBindableHotkey bhk)
                        bhk.PropertyChanged += this.HotkeyOnPropertyChanged;
                }));
                break;
            default: break;
            }
        }

#       pragma warning disable CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        /// <summary>Triggered after the value changes when a property's setter is called.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        #endregion Events

        #region Properties
        private static Config Settings => (AppConfig.Configuration.Default as Config)!;
        private static LogWriter Log => FLog.Log;
        /// <summary>
        /// Action manager object
        /// </summary>
        public IHotkeyActionManager Actions { get; }
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
                this.AllSelectedChanged();
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
            this.Hotkeys.Add(hk);
            Log.Info($"Created a new hotkey entry:", JsonConvert.SerializeObject(hk));
            this.RecheckAllSelected();
        }
        /// <summary>
        /// Create a new blank hotkey and add it to <see cref="Hotkeys"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="BindableHotkey.Name"/> = <see cref="string.Empty"/>
        /// </remarks>
        public void AddHotkey() => this.AddHotkey(new BindableHotkey());
        /// <summary>
        /// Remove the specified hotkey from <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="hk">The <see cref="Hotkey"/> object to remove.<br/>If this is null, nothing happens.</param>
        public void DelHotkey(IBindableHotkey? hk)
        {
            if (hk == null)
                return;
            _ = this.Hotkeys.Remove(hk);
            Log.Info($"Deleted hotkey {hk.ID} '{hk.Name}'");
            this.RecheckAllSelected();
        }
        /// <summary>
        /// Remove the specified hotkey from <see cref="Hotkeys"/>.
        /// </summary>
        /// <param name="id">The Windows API ID number of the hotkey to delete.</param>
        public void DelHotkey(int id)
        {
            for (int i = this.Hotkeys.Count - 1; i >= 0; --i)
            {
                if (this.Hotkeys[i].ID.Equals(id))
                {
                    this.Hotkeys[i].Registered = false;
                    this.Hotkeys[i].Dispose();
                    this.Hotkeys.RemoveAt(i);
                }
            }
            this.RecheckAllSelected();
        }
        /// <summary>
        /// Deletes all hotkeys in the list by first disposing them, then removing them from the list.
        /// </summary>
        public void DelAllHotkeys()
        {
            for (int i = this.Hotkeys.Count - 1; i >= 0; --i)
            {
                this.Hotkeys[i].Registered = false;
                this.Hotkeys[i].Dispose();
                this.Hotkeys.RemoveAt(i);
            }
            this.RecheckAllSelected();
        }
        #endregion HotkeysListManipulators

        #region HotkeysListGetters
        /// <summary>
        /// Gets a single <see cref="Hotkey"/> from the list by checking for matching <paramref name="id"/> numbers.
        /// </summary>
        /// <param name="id">The ID number of the target hotkey.<br/><i>(This is compared to the <see cref="Hotkey.ID"/> property.)</i></param>
        /// <returns>The target <see cref="Hotkey"/> instance if successful; otherwise <see langword="null"/>.</returns>
        public IBindableHotkey? GetHotkey(int id) => this.Hotkeys.FirstOrDefault(hk => hk is not null && hk.ID.Equals(id), null);
        #endregion HotkeysListGetters

        #region HotkeysListSaveLoad
        /// <summary>
        /// Loads hotkeys from the settings file and binds them to the associated actions.
        /// </summary>
        /// <remarks><b>Make sure that the <see cref="Actions"/> property is set and initialized before calling this!</b></remarks>
        public void LoadHotkeys()
        {
            _ = this.Hotkeys.AddRangeIfUnique((Settings.Hotkeys ??= Config.Hotkeys_Default).ConvertEach(hkjw => hkjw.ToBindableHotkey(this.Actions)));
            this.RecheckAllSelected();
        }
        /// <summary>
        /// Saves all hotkeys to the settings file.
        /// </summary>
        public void SaveHotkeys()
        {
            Settings.Hotkeys = this.Hotkeys.ConvertEach(hk => hk.ToBindableHotkeyJsonWrapper()).ToArray();
            //Settings.Save();
        }
        /// <summary>Resets the current hotkey list by replacing it with <see cref="Config.Hotkeys_Default"/>.</summary>
        public void ResetHotkeys()
        {
            this.DelAllHotkeys();
            Settings.Hotkeys = null!;
            WindowsHotkeyAPI.ResetIDs();
            this.LoadHotkeys();
        }
        #endregion HotkeysListSaveLoad

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.SaveHotkeys(); //< this saves hotkeys to the settings file
                    this.DelAllHotkeys(); //< this cleans up Windows API hotkey registrations
                }

                disposedValue = true;
            }
        }
        /// <summary>Finalizer</summary>
        ~HotkeyManager() { this.Dispose(disposing: false); }
        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable

        #region AllSelected
        private void HotkeyOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            // Only re-check if the IsChecked property changed
            if (args.PropertyName == nameof(Hotkey.Registered))
                this.RecheckAllSelected();
        }
        private void AllSelectedChanged()
        {
            // Has this change been caused by some other change?
            // return so we don't mess things up
            if (_allSelectedChanging) return;

            try
            {
                Settings.PauseAutoSave(); //< prevent writing changes all at once
                _allSelectedChanging = true;

                // this can of course be simplified
                if (this.AllSelected == true)
                {
                    foreach (IBindableHotkey hk in this.Hotkeys)
                        hk.Registered = true;
                }
                else if (this.AllSelected == false)
                {
                    foreach (IBindableHotkey hk in this.Hotkeys)
                        hk.Registered = false;
                }
            }
            finally
            {
                Log.Debug($"All hotkeys were {(this.AllSelected ?? false ? string.Empty : "un")}registered, saving config...");
                Settings.Save();
                Settings.ResumeAutoSave();
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

                if (this.Hotkeys.Count > 0)
                {
                    bool prev = this.Hotkeys.First().Registered;
                    bool fullLoop = true;
                    for (int i = 1; i < this.Hotkeys.Count; ++i)
                    {
                        if (this.Hotkeys[i].Registered != prev)
                        {
                            fullLoop = false;
                            this.AllSelected = null;
                            break;
                        }
                    }
                    if (fullLoop)
                        this.AllSelected = prev;
                }
                else
                {
                    this.AllSelected = false;
                }
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
