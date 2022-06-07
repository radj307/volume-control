using HotkeyLib;
using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Hotkeys.Structs;

namespace VolumeControl.Hotkeys.Interfaces
{
    /// <summary>
    /// Represents a hotkey action manager object, which contains the list of <see cref="IActionBinding"/>s used in the hotkey system.
    /// </summary>
    public interface IHotkeyActionManager : IBaseAddon, INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>The list of known hotkey action bindings.</summary>
        List<IActionBinding> Bindings { get; }
        /// <summary>
        /// Gets the <see cref="KeyEventHandler"/> associated with <paramref name="actionName"/>.
        /// </summary>
        /// <param name="actionName">The name of the target action.</param>
        /// <returns>The <see cref="KeyEventHandler"/> associated with the specified name, or <see cref="NullAction"/> if no actions by that name were found.</returns>
        IActionBinding this[string actionName] { get; }
        /// <summary>Gets a list of all of the <see cref="IActionBinding.Name"/> properties currently contained within the <see cref="Bindings"/> list.</summary>
        /// <returns>List of <see cref="IActionBinding.Name"/> properties.</returns>
        List<string> GetActionNames();
        /// <summary>This is the default action handler, which does nothing.</summary>
        public static readonly KeyEventHandler NullActionHandler = delegate { };
        /// <summary>This is the default action, which does nothing.</summary>
        public static readonly IActionBinding NullAction = new ActionBinding(NullActionHandler.Method, null, new("None"));
    }
}
