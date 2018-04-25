using JetBrains.Annotations;
using System;
using Toastify.Core;
using ToastifyAPI.Events;
using ToastifyAPI.Native.Enums;

namespace Toastify.Model
{
    public sealed class ToastifyVolumeMute : ToastifyVolumeAction
    {
        private readonly Action spotifyOnlyVolumeAction;

        /// <inheritdoc />
        public override string Name
        {
            get { return "Toggle Mute"; }
        }

        /// <inheritdoc />
        public override ToastifyActionEnum ToastifyActionEnum
        {
            get { return ToastifyActionEnum.Mute; }
        }

        /// <inheritdoc />
        public override long AppCommandCode { get; } = 0x00080000L;

        /// <inheritdoc />
        public override VirtualKeyCode VirtualKeyCode { get; } = VirtualKeyCode.VK_VOLUME_MUTE;

        /// <inheritdoc />
        public ToastifyVolumeMute([NotNull] GetVolumeControlModeDelegate getVolumeControlModeDelegate, [NotNull] Action spotifyOnlyVolumeAction) : base(getVolumeControlModeDelegate)
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