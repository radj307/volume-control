using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Hotkeys.Structs;

namespace VolumeControl.Hotkeys.Interfaces
{
    /// <summary>
    /// Represents a hotkey action manager object, which contains the list of <see cref="IActionBinding"/>s used in the hotkey system.
    /// </summary>
    public interface IHotkeyActionManager : IBaseAddon, INotifyPropertyChanged
    {
        /// <summary>The list of known hotkey action bindings.</summary>
        List<IActionBinding> Bindings { get; }
        /// <summary>
        /// Gets the <see cref="IActionBinding"/> associated with <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier of the target action.</param>
        /// <returns>The <see cref="IActionBinding"/> with the specified <paramref name="identifier"/>.</returns>
        IActionBinding this[string identifier] { get; }
        /// <summary>This is the default action handler, which does nothing.</summary>
        public static readonly HandledEventHandler NullActionHandler = delegate { };
        /// <summary>This is the default action, which does nothing.</summary>
        public static readonly IActionBinding NullAction = new ActionBinding(NullActionHandler.Method, null, new("None", null, null));
    }
}
