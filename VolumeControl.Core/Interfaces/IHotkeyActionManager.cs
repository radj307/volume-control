using System.ComponentModel;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Structs;

namespace VolumeControl.Core.Interfaces
{
    /// <summary>
    /// Represents a hotkey action manager object, which contains the list of <see cref="IHotkeyAction"/>s used in the hotkey system.
    /// </summary>
    public interface IHotkeyActionManager : INotifyPropertyChanged
    {
        /// <summary>The list of known hotkey action bindings.</summary>
        List<IHotkeyAction> Bindings { get; }
        /// <summary>
        /// Gets the <see cref="IHotkeyAction"/> associated with <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier of the target action.</param>
        /// <returns>The <see cref="IHotkeyAction"/> with the specified <paramref name="identifier"/>.</returns>
        IHotkeyAction this[string identifier] { get; }
        /// <summary>This is the default action handler, which does nothing.</summary>
        public static readonly HandledEventHandler NullActionHandler = delegate { };
        /// <summary>This is the default action, which does nothing.</summary>
        public static readonly IHotkeyAction NullAction = new HotkeyAction(NullActionHandler.Method, NullActionHandler.Method, new("None", null, null));
    }
}
