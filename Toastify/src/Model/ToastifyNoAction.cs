using Toastify.Core;

namespace Toastify.Model
{
    public sealed class ToastifyNoAction : ToastifyAction
    {
        #region Public properties

        /// <inheritdoc />
        public override string Name
        {
            get { return "No Action"; }
        }

        /// <inheritdoc />
        public override ToastifyActionEnum ToastifyActionEnum
        {
            get { return ToastifyActionEnum.None; }
        }

        #endregion

        /// <inheritdoc />
        public override void PerformAction()
        {
        }
    }
}