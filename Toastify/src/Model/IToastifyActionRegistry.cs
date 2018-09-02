using JetBrains.Annotations;
using Toastify.Core;

namespace Toastify.Model
{
    /// <summary>
    ///     Supports the retrieval of <see cref="ToastifyAction" /> instances from a registry of <see cref="ToastifyActionEnum" /> values.
    /// </summary>
    public interface IToastifyActionRegistry
    {
        [CanBeNull]
        ToastifyAction GetAction(ToastifyActionEnum actionEnum);
    }
}