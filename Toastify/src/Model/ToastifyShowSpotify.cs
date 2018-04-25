using Toastify.Core;

namespace Toastify.Model
{
    public sealed class ToastifyShowSpotify : ToastifyAction
    {
        /// <inheritdoc />
        public override string Name
        {
            get { return "Show Spotify"; }
        }

        /// <inheritdoc />
        public override ToastifyActionEnum ToastifyActionEnum
        {
            get { return ToastifyActionEnum.ShowSpotify; }
        }

        /// <inheritdoc />
        public override void PerformAction()
        {
            if (Spotify.Instance.IsMinimized)
                Spotify.Instance.ShowSpotify();
            else
                Spotify.Instance.Minimize();

            this.RaiseActionPerformed(this);
        }
    }
}