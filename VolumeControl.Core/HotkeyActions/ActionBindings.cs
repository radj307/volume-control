using HotkeyLib;
using System.Reflection;
using VolumeControl.Core.HotkeyActions.Attributes;
using VolumeControl.Core.HotkeyActions.Interfaces;
using VolumeControl.Core.HotkeyActions.Structs;

namespace VolumeControl.Core.HotkeyActions
{

    /// <summary>
    /// This is the manager object for hotkey-action bindings.
    /// </summary>
    /// <remarks>This object handles using reflection to identify action handlers.</remarks>
    public class ActionBindings
    {
        public ActionBindings(params object?[] methodContainers)
        {
            Bindings = new()
            {
                new ActionBinding(NullAction.GetMethodInfo(), null, new HandlerAttribute("None"))
            };

            foreach (var hObj in methodContainers)
            {
                if (hObj == null)
                    continue;
                MergeActionBindingsList(ParseActionMethods(hObj));
            }
        }

        /// <summary>
        /// Parses the given class to retrieve a list containing all of its public methods marked with <see cref="HandlerAttribute"/>.
        /// </summary>
        /// <param name="handlerObject">Any class type with public handler method definitions.</param>
        /// <returns>A list of all of the public methods from <paramref name="handlerObject"/> that are marked with <see cref="HandlerAttribute"/>.</returns>
        private static List<IActionBinding> ParseActionMethods(object handlerObject)
        {
            Type type = handlerObject.GetType();
            List<IActionBinding> bindings = new();

            foreach (MethodInfo m in type.GetMethods())
            {
                if (m.GetCustomAttribute(typeof(HandlerAttribute)) is HandlerAttribute hAttr)
                    bindings.Add(new ActionBinding(m, handlerObject, hAttr));
            }

            return bindings;
        }

        private void MergeActionBindingsList(List<IActionBinding> list)
        {
            foreach (var action in list)
            {
                if (Bindings.Any(b => b.Name.Equals(action.Name, StringComparison.OrdinalIgnoreCase)))
                    throw new Exception($"An action binding with the name '{action.Name}' already exists!");
                Bindings.Add(action);
            }
        }

        public List<IActionBinding> Bindings { get; private set; }
        public KeyEventHandler this[string actionName]
        {
            get
            {
                for (int i = 0; i < Bindings.Count; ++i)
                {
                    if (Bindings[i].Name.Equals(actionName, StringComparison.Ordinal))
                        return Bindings[i].HandleKeyEvent;
                }
                return NullAction;
            }
        }

        /// <summary>
        /// Gets a list of all of the <see cref="IActionBinding.Name"/> properties currently contained within the <see cref="Bindings"/> list.
        /// </summary>
        /// <returns>List of <see cref="IActionBinding.Name"/> properties.</returns>
        public List<string> GetActionNames() => Bindings.Select(i => i.Name).ToList();

        /// <summary>
        /// This is the default action, which does nothing.
        /// </summary>
        protected static readonly KeyEventHandler NullAction = delegate { };
    }
}
