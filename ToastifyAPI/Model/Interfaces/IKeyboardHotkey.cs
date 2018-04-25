using System.Windows.Input;

namespace ToastifyAPI.Model.Interfaces
{
    public interface IKeyboardHotkey : IHotkey
    {
        Key? Key { get; set; }
    }
}