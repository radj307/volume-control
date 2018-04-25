using Toastify.Core;

namespace Toastify.Model
{
    /// <summary>
    /// Terminates Toastify.
    /// </summary>
    public sealed class ToastifyExit : ToastifyAction
    {
        /// <inheritdoc />
        public override string Name
        {
            get { return "Exit"; }
        }

        /// <inheritdoc />
        public override ToastifyActionEnum ToastifyActionEnum
        {
            get { return ToastifyActionEnum.Exit; }
        }

        /// <inheritdoc />
        public override void PerformAction()
        {
            App.Terminate();
        }
    }
}