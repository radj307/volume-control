using System.ComponentModel;
using ManagedWinapi;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Model
{
    public class GlobalHotkey : Hotkey, IGlobalHotkey, ILockedGlobalHotkey
    {
    }
}