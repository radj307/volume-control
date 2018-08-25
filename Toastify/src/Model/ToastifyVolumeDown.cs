using System;
using JetBrains.Annotations;
using Toastify.Core;
using ToastifyAPI.Events;
using ToastifyAPI.Native.Enums;

namespace Toastify.Model
{
    public sealed class ToastifyVolumeDown : ToastifyVolumeAction
    {
        private readonly Action spotifyOnlyVolumeAction;

        #region Public Properties

        /// <inheritdoc />
        public override string Name
        {
            get { return "Volume Down"; }
        }

        /// <inheritdoc />
        public override ToastifyActionEnum ToastifyActionEnum
        {
            get { return ToastifyActionEnum.VolumeDown; }
        }

        /// <inheritdoc />
        public override long AppCommandCode { get; } = 0x00090000L;

        /// <inheritdoc />
        public override VirtualKeyCode VirtualKeyCode { get; } = VirtualKeyCode.VK_VOLUME_DOWN;

        #endregion

        /// <inheritdoc />
        public ToastifyVolumeDown([NotNull] GetVolumeControlModeDelegate getVolumeControlModeDelegate, [NotNull] Action spotifyOnlyVolumeAction) : base(getVolumeControlModeDelegate)
        {
            this.spotifyOnlyVolumeAction = spotifyOnlyVolumeAction;
        }

        /// <inheritdoc />
        protected override void PerformSystemGlobalAction()
        {
            this.PerformMediaAction();
        }

        /// <inheritdoc />
        protected override void PerformSystemSpotifyOnlyAction()
        {
            if (this.spotifyOnlyVolumeAction == null)
                this.RaiseActionFailed(this, new ActionFailedEventArgs("SpotifyLocalAPI is null."));
            else
            {
                this.spotifyOnlyVolumeAction.Invoke();
                this.RaiseActionPerformed(this);
            }
        }
    }
}