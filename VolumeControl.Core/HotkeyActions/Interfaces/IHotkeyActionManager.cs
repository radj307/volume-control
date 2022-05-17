using HotkeyLib;

namespace VolumeControl.Core.HotkeyActions.Interfaces
{
    public interface IHotkeyActionManager
    {
        /// <summary>The list of known hotkey action bindings.</summary>
        List<IActionBinding> Bindings { get; }
        /// <summary>
        /// Gets the <see cref="KeyEventHandler"/> associated with <paramref name="actionName"/>.
        /// </summary>
        /// <param name="actionName">The name of the target action.</param>
        /// <returns>The <see cref="KeyEventHandler"/> associated with the specified name, or <see cref="NullAction"/> if no actions by that name were found.</returns>
        KeyEventHandler this[string actionName] { get; }

        /// <summary>Gets a list of all of the <see cref="IActionBinding.Name"/> properties currently contained within the <see cref="Bindings"/> list.</summary>
        /// <returns>List of <see cref="IActionBinding.Name"/> properties.</returns>
        List<string> GetActionNames();
        /// <summary>This is the default action, which does nothing.</summary>
        public static readonly KeyEventHandler NullAction = delegate { };
    }
}
