using Toastify.Core;

namespace Toastify.Model
{
    /// <summary>
    ///     Void action
    /// </summary>
    public sealed class ToastifyNoAction : ToastifyAction
    {
        #region Static Fields and Properties

        public static readonly string ActionName = "No Action";

        #endregion

        #region Public Properties

        /// <inheritdoc />
        public override string Name
        {
            get { return ActionName; }
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
            // No action
        }
    }
}