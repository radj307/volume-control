using Toastify.Core;

namespace Toastify.Model
{
    public interface IToastifyActionRegistry
    {
        ToastifyAction GetAction(ToastifyActionEnum actionEnum);
    }
}