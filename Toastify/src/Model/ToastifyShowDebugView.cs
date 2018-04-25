using Toastify.Core;
using Toastify.View;

namespace Toastify.Model
{
    public sealed class ToastifyShowDebugView : ToastifyAction
    {
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

        /// <inheritdoc />
        public override void PerformAction()
        {
#if DEBUG
            if (DebugView.Current == null)
                DebugView.Launch();
#endif
        }
    }
}