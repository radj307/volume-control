using HotkeyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using VolumeControl.Core;
using VolumeControl.Hotkeys;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Hotkeys.Interfaces;
using VolumeControl.Hotkeys.Structs;
using VolumeControl.Log;

namespace VolumeControl.Helpers
{
    /// <summary>Hotkey action management object that uses reflection to automatically set up hotkey actions from a given list of objects.<br/>See the </summary>
    /// <remarks>You can override this object to modify how it works; pass the derived object to <see cref="HotkeyManager(IHotkeyActionManager)"/></remarks>
    public class HotkeyActionManager : BaseAddon, IHotkeyActionManager, INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <inheritdoc cref="HotkeyActionManager">
        public HotkeyActionManager() : base(typeof(ActionAddonAttribute))
        {
            // initialize the bindings list
            _bindings = null!;//< quiet compiler, use Bindings anyway so the events trigger
            Bindings = new() { IHotkeyActionManager.NullAction };
        }

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        public event EventHandler? BindingsUpdated;
        protected virtual void NotifyBindingsUpdated() => BindingsUpdated?.Invoke(this, new());

        public event PropertyChangingEventHandler? PropertyChanging;
        protected virtual void NotifyPropertyChanging([CallerMemberName] string propertyName = "") => PropertyChanging?.Invoke(this, new(propertyName));
        #endregion Events

        /// <summary>
        /// Recursive method that populates the <see cref="Bindings"/> list from any enumerable object container. Any sub-enumerable types are recursed into.
        /// </summary>
        private void GetActionMethods()
        {
            foreach (Type t in Types)
            {
                try
                {
                    var obj = Activator.CreateInstance(t);
                    if (obj == null)
                    {
                        FLog.Log.Error($"Type instantiation failed: {t.FullName}");
                        continue;
                    }
                    MergeActionBindingsList(ParseActionMethods(obj));
                }
                catch (Exception ex)
                {
                    FLog.Log.Error($"An exception was thrown while instantiating an object of type {t.FullName}!",ex);
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
                    bindings.Add(new ActionBinding(m, handlerObject, hAttr));
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
                if (Bindings.Any(b => b.Name.Equals(action.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    switch (resolutionType)
                    {
                    case NameConflictResolution.Throw:
                        throw new Exception($"An action binding with the name '{action.Name}' already exists!");
                    case NameConflictResolution.Ignore:
                        continue; //< continue to next element
                    case NameConflictResolution.Overwrite:
                        break; //< break to add action
                    }
                }
                NotifyPropertyChanging(nameof(Bindings));
                Bindings.Add(action);
                NotifyPropertyChanged(nameof(Bindings));
            }
        }

        /// <inheritdoc/>
        public List<IActionBinding> Bindings
        {
            get => _bindings;
            private set
            {
                NotifyPropertyChanging();
                _bindings = value;
                NotifyPropertyChanged();
            }
        }
        private List<IActionBinding> _bindings;
        /// <inheritdoc/>
        public IActionBinding this[string actionName]
        {
            get
            {
                for (int i = 0; i < Bindings.Count; ++i)
                {
                    if (Bindings[i].Name.Equals(actionName, StringComparison.Ordinal))
                        return Bindings[i];
                }
                return IHotkeyActionManager.NullAction;
            }
        }
        /// <inheritdoc/>
        public List<string> GetActionNames() => Bindings.Select(i => i.Name).ToList();
        /// <inheritdoc/>
        public override void LoadFromTypes() => GetActionMethods();
    }
}
