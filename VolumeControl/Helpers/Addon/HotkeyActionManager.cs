using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Core.Keyboard.Actions;
using VolumeControl.Hotkeys;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Hotkeys.Interfaces;
using VolumeControl.Hotkeys.Structs;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Addon
{
    /// <summary>
    /// Manages the list of hotkey 'actions' available to the hotkey binding system.<br/>
    /// The action itself is parsed from, and created in conjunction with, class methods marked with <see cref="HotkeyActionAttribute"/>.
    /// </summary>
    /// <remarks>You can override this object to modify how it works; pass the derived object to <see cref="HotkeyManager(IHotkeyActionManager)"/></remarks>
    public class HotkeyActionManager : BaseAddon, IHotkeyActionManager, INotifyPropertyChanged
    {
        #region Constructor
        /// <inheritdoc cref="HotkeyActionManager">
        public HotkeyActionManager() : base(typeof(ActionAddonAttribute)) => this.Bindings = new() { IHotkeyActionManager.NullAction };
        #endregion Constructor

        #region Events
#       pragma warning disable CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067 // The event 'BindableHotkey.PropertyChanged' is never used ; This is automatically used by Fody.

        public event EventHandler? BindingsUpdated;
        protected virtual void NotifyBindingsUpdated() => BindingsUpdated?.Invoke(this, new());
        #endregion Events

        #region Methods
        /// <summary>
        /// Recursive method that populates the <see cref="Bindings"/> list from any enumerable object container. Any sub-enumerable types are recursed into.
        /// </summary>
        private void GetActionMethods()
        {
            foreach (Type t in this.Types)
            {
                try
                {
                    object? obj = Activator.CreateInstance(t);
                    if (obj == null)
                    {
                        FLog.Log.Error($"Type instantiation failed: {t.FullName}");
                        continue;
                    }
                    this.MergeActionBindingsList(ParseActionMethods(obj));
                }
                catch (Exception ex)
                {
                    FLog.Log.Error($"An exception was thrown while instantiating an object of type {t.FullName}!", ex);
                }
            }
        }
        /// <summary>
        /// Parses the given class to retrieve a list containing all of its public methods marked with <see cref="HotkeyActionAttribute"/>.
        /// </summary>
        /// <param name="handlerObject">Any class type with public handler method definitions.</param>
        /// <returns>A list of all of the public methods from <paramref name="handlerObject"/> that are marked with <see cref="HotkeyActionAttribute"/>.</returns>
        protected static List<IActionBinding> ParseActionMethods(object handlerObject)
        {
            Type type = handlerObject.GetType();
            List<IActionBinding> bindings = new();

            foreach (MethodInfo m in type.GetMethods())
            {
                if (m.GetCustomAttribute(typeof(HotkeyActionAttribute)) is HotkeyActionAttribute hAttr)
                    bindings.Add(new ActionBinding(m, handlerObject, hAttr.GetActionData()));
            }

            return bindings;
        }
        /// <summary>Determines how duplicated action names are handled by the parser.</summary>
        protected enum NameConflictResolution
        {
            /// <summary>When an action with a conflicting name is found, an exception is thrown that includes the <see cref="IActionBinding.Name"/> of the duplicate action.</summary>
            Throw,
            /// <summary>When an action with a conflicting name is found, it is ignored.</summary>
            Ignore,
            /// <summary>When an action with a conflicting name is found, it will overwrite the action already in the <see cref="Bindings"/> list.</summary>
            Overwrite,
        }
        /// <summary>
        /// Merges a list of <see cref="IActionBinding"/> interface types into the <see cref="Bindings"/> list property.
        /// </summary>
        /// <param name="list">List of action bindings to merge.</param>
        /// <param name="resolutionType">Determines how to handle duplicated action names.</param>
        /// <exception cref="Exception">An action with the same name already exists.</exception>
        protected virtual void MergeActionBindingsList(List<IActionBinding> list, NameConflictResolution resolutionType = NameConflictResolution.Throw)
        {
            foreach (IActionBinding? action in list)
            {
                foreach (IActionBinding? bound in this.Bindings)
                {
                    if ((bound.Data.ActionGroup?.Equals(action.Data.ActionGroup, StringComparison.Ordinal) ?? false) && bound.Data.ActionName.Equals(action.Data.ActionName, StringComparison.Ordinal))
                    { // action is a duplicate:
                        switch (resolutionType)
                        {
                        case NameConflictResolution.Throw:
                            throw new Exception($"Action Name Collision Detected in Group '{action.Data.ActionGroup ?? "null"}': '{action.Data.ActionName}'!");
                        case NameConflictResolution.Overwrite:
                            break;
                        case NameConflictResolution.Ignore:
                            continue;
                        }
                    }
                }
                // Insert new action
                this.Bindings.Add(action);
            }
        }
        /// <inheritdoc/>
        public List<IActionBinding> Bindings { get; set; }
        /// <inheritdoc/>
        [SuppressPropertyChangedWarnings]
        public IActionBinding this[string identifier]
        {
            get
            {
                for (int i = 0; i < this.Bindings.Count; ++i)
                {
                    if (this.Bindings[i].Identifier.Equals(identifier, StringComparison.Ordinal))
                        return this.Bindings[i];
                }
                return IHotkeyActionManager.NullAction;
            }
            set
            {
                if (this.Bindings.FirstOrDefault(b => b is not null && b.Identifier.Equals(identifier, StringComparison.Ordinal), null) is IActionBinding actionBinding)
                    actionBinding = value;
            }
        }
        /// <inheritdoc/>
        public override void LoadFromTypes() => this.GetActionMethods();
        #endregion Methods
    }
}
