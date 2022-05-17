using HotkeyLib;
using System.Reflection;
using VolumeControl.Core.HotkeyActions.Attributes;
using VolumeControl.Core.HotkeyActions.Interfaces;
using VolumeControl.Core.HotkeyActions.Structs;

namespace VolumeControl.Core.HotkeyActions
{
    /// <summary>
    /// Hotkey action management object that uses reflection to automatically set up hotkey actions from a given list of objects.<br/>See the 
    /// </summary>
    /// <remarks>You can override this object to modify how it works; pass the derived object to <see cref="HotkeyManager(IHotkeyActionManager)"/></remarks>
    public class HotkeyActionManager : IHotkeyActionManager
    {
        /// <inheritdoc cref="HotkeyActionManager">
        /// <param name="methodContainers">This is a variadic list of objects that contain methods marked with <see cref="HotkeyActionAttribute"/> to parse bindings from.</param>
        public HotkeyActionManager(params object?[] methodContainers)
        {
            // initialize the bindings list
            Bindings = new() { new ActionBinding(IHotkeyActionManager.NullAction.GetMethodInfo(), null, new HotkeyActionAttribute("None")) };
            // parse action methods from the given objects
            foreach (var hObj in methodContainers)
            {
                if (hObj == null)
                    continue;
                MergeActionBindingsList(ParseActionMethods(hObj));
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
            foreach (var action in list)
            {
                if (Bindings.Any(b => b.Name.Equals(action.Name, StringComparison.OrdinalIgnoreCase)))
                    switch (resolutionType)
                    {
                    case NameConflictResolution.Throw:
                        throw new Exception($"An action binding with the name '{action.Name}' already exists!");
                    case NameConflictResolution.Ignore:
                        continue; //< continue to next element
                    case NameConflictResolution.Overwrite:
                        break; //< break to add action
                    }
                Bindings.Add(action);
            }
        }

        /// <inheritdoc/>
        public List<IActionBinding> Bindings { get; private set; }
        /// <inheritdoc/>
        public KeyEventHandler this[string actionName]
        {
            get
            {
                for (int i = 0; i < Bindings.Count; ++i)
                {
                    if (Bindings[i].Name.Equals(actionName, StringComparison.Ordinal))
                        return Bindings[i].HandleKeyEvent;
                }
                return IHotkeyActionManager.NullAction;
            }
        }
        /// <inheritdoc/>
        public List<string> GetActionNames() => Bindings.Select(i => i.Name).ToList();
    }
}
