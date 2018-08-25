using Toastify.Core;
using ToastifyAPI.Native.Enums;

namespace Toastify.Model
{
    public class ToastifySimpleMediaAction : ToastifyMediaAction
    {
        public ToastifySimpleMediaAction(string name, ToastifyActionEnum actionEnum, long appCommandCode) : base(name, actionEnum, appCommandCode)
        {
        }

        public ToastifySimpleMediaAction(string name, ToastifyActionEnum actionEnum, long appCommandCode, VirtualKeyCode virtualKeyCode) : base(name, actionEnum, appCommandCode, virtualKeyCode)
        {
        }

        /// <inheritdoc />
        public override void PerformAction()
        {
            this.PerformMediaAction();
        }
    }
}