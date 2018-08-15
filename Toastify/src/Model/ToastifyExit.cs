using Toastify.Core;

namespace Toastify.Model
{
    /// <summary>
    ///     Terminates Toastify.
    /// </summary>
    public sealed class ToastifyExit : ToastifyAction
    {
        #region Public properties

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

        #endregion

        /// <inheritdoc />
        public override void PerformAction()
        {
            App.Terminate();
        }
    }
}