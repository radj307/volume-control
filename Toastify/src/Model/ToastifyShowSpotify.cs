using Toastify.Core;

namespace Toastify.Model
{
    /// <summary>
    ///     This action shows or minimizes Spotify's main window.
    /// </summary>
    public sealed class ToastifyShowSpotify : ToastifyAction
    {
        #region Public Properties

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

        #endregion

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