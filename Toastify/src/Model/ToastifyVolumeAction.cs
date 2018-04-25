using JetBrains.Annotations;
using System;
using Toastify.Core;
using ToastifyAPI.Events;

namespace Toastify.Model
{
    public abstract class ToastifyVolumeAction : ToastifyMediaAction
    {
        private readonly GetVolumeControlModeDelegate getVolumeControlMode;

        protected ToastifyVolumeAction([NotNull] GetVolumeControlModeDelegate getVolumeControlModeDelegate)
        {
            this.getVolumeControlMode = getVolumeControlModeDelegate;
        }

        public override void PerformAction()
        {
            ToastifyVolumeControlMode mode = this.GetVolumeControlMode();
            switch (mode)
            {
                case ToastifyVolumeControlMode.SystemGlobal:
                    this.PerformSystemGlobalAction();
                    break;

                case ToastifyVolumeControlMode.SystemSpotifyOnly:
                    this.PerformSystemSpotifyOnlyAction();
                    break;

                default:
                    this.RaiseActionFailed(this, new ActionFailedEventArgs($"Unhandled {nameof(ToastifyVolumeControlMode)} value: {mode}"));
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract void PerformSystemGlobalAction();

        protected abstract void PerformSystemSpotifyOnlyAction();

        private ToastifyVolumeControlMode GetVolumeControlMode()
        {
            return this.getVolumeControlMode.Invoke();
        }
    }
}