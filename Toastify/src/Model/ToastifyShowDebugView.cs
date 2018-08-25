#if DEBUG

using Toastify.Core;
using Toastify.View;

namespace Toastify.Model
{
    public sealed class ToastifyShowDebugView : ToastifyAction
    {
        #region Public Properties

        /// <inheritdoc />
        public override string Name
        {
            get { return "Show DebugView"; }
        }

        /// <inheritdoc />
        public override ToastifyActionEnum ToastifyActionEnum
        {
            get { return ToastifyActionEnum.ShowDebugView; }
        }

        #endregion

        /// <inheritdoc />
        public override void PerformAction()
        {
            if (DebugView.Current == null)
                DebugView.Launch();
        }
    }
}

#endif