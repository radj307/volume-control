using System.ComponentModel;

namespace VolumeControl.SDK.Delegates
{
    /// <summary>
    /// Represents a method that can be used by Volume Control to generate a hotkey action.
    /// </summary>
    /// <param name="sender">The <see cref="Hotkeys."/></param>
    /// <param name="e"></param>
    public delegate void HotkeyActionDelegate(object? sender, HandledEventArgs e);
    public delegate void HotkeyActionDelegate<T1>(object? sender, HandledEventArgs e, T1 opt1);
    public delegate void HotkeyActionDelegate<T1, T2>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2);
    public delegate void HotkeyActionDelegate<T1, T2, T3>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2, T3 opt3);
    public delegate void HotkeyActionDelegate<T1, T2, T3, T4>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2, T3 opt3, T4 opt4);
    public delegate void HotkeyActionDelegate<T1, T2, T3, T4, T5>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2, T3 opt3, T4 opt4, T5 opt5);
    public delegate void HotkeyActionDelegate<T1, T2, T3, T4, T5, T6>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2, T3 opt3, T4 opt4, T5 opt5, T6 opt6);
    public delegate void HotkeyActionDelegate<T1, T2, T3, T4, T5, T6, T7>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2, T3 opt3, T4 opt4, T5 opt5, T6 opt6, T7 opt7);
    public delegate void HotkeyActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2, T3 opt3, T4 opt4, T5 opt5, T6 opt6, T7 opt7, T8 opt8);
    public delegate void HotkeyActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9>(object? sender, HandledEventArgs e, T1 opt1, T2 opt2, T3 opt3, T4 opt4, T5 opt5, T6 opt6, T7 opt7, T8 opt8, T9 opt9);
}
