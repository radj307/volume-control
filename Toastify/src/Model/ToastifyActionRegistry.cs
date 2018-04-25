using log4net;
using System.Collections.Generic;
using Toastify.Core;
using ToastifyAPI.Native.Enums;

namespace Toastify.Model
{
    public class ToastifyActionRegistry : IToastifyActionRegistry
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyActionRegistry));

        private readonly IDictionary<ToastifyActionEnum, ToastifyAction> actions = new Dictionary<ToastifyActionEnum, ToastifyAction>();

        /// <inheritdoc />
        public ToastifyAction GetAction(ToastifyActionEnum actionEnum)
        {
            return this.GetOrAddAction(actionEnum);
        }

        private ToastifyAction GetOrAddAction(ToastifyActionEnum actionEnum)
        {
            if (this.actions.ContainsKey(actionEnum))
                return this.actions[actionEnum];

            ToastifyAction action = null;
            ToastifyVolumeControlMode GetVolumeControlModeDelegate() => Settings.Current.VolumeControlMode;

            switch (actionEnum)
            {
                case ToastifyActionEnum.None:
                    action = new ToastifyNoAction();
                    break;

                case ToastifyActionEnum.ShowToast:
#if DEBUG
                    action = new ToastifyShowToast { ShouldPrintExtendedDebugLog = true };
#else
                    action = new ToastifyShowToast();
#endif
                    break;

                case ToastifyActionEnum.ShowSpotify:
                    action = new ToastifyShowSpotify();
                    break;

                case ToastifyActionEnum.PlayPause:
                    action = new ToastifySimpleMediaAction("Play / Pause", actionEnum, 0x000E0000L, VirtualKeyCode.VK_MEDIA_PLAY_PAUSE)
                    {
                        ActionType = MediaActionType.AppCommandMessage
                    };
                    break;

                case ToastifyActionEnum.Mute:
                    action = new ToastifyVolumeMute(GetVolumeControlModeDelegate, () => Spotify.Instance.ToggleMute())
                    {
                        ActionType = MediaActionType.MediaKey
                    };
                    break;

                case ToastifyActionEnum.VolumeDown:
                    action = new ToastifyVolumeDown(GetVolumeControlModeDelegate, () => Spotify.Instance.VolumeDown())
                    {
                        ActionType = MediaActionType.MediaKey
                    };
                    break;

                case ToastifyActionEnum.VolumeUp:
                    action = new ToastifyVolumeUp(GetVolumeControlModeDelegate, () => Spotify.Instance.VolumeUp())
                    {
                        ActionType = MediaActionType.MediaKey
                    };
                    break;

                case ToastifyActionEnum.PreviousTrack:
                    action = new ToastifySimpleMediaAction("Previous Track", actionEnum, 0x000C0000L, VirtualKeyCode.VK_MEDIA_PREV_TRACK)
                    {
                        ActionType = MediaActionType.AppCommandMessage
                    };
                    break;

                case ToastifyActionEnum.NextTrack:
                    action = new ToastifySimpleMediaAction("Next Track", actionEnum, 0x000B0000L, VirtualKeyCode.VK_MEDIA_NEXT_TRACK)
                    {
                        ActionType = MediaActionType.AppCommandMessage
                    };
                    break;

                case ToastifyActionEnum.FastForward:
                    action = new ToastifySimpleMediaAction("Fast Forward", actionEnum, 0x00310000L)
                    {
                        ActionType = MediaActionType.AppCommandMessage
                    };
                    break;

                case ToastifyActionEnum.Rewind:
                    action = new ToastifySimpleMediaAction("Rewind", actionEnum, 0x00320000L)
                    {
                        ActionType = MediaActionType.AppCommandMessage
                    };
                    break;

                case ToastifyActionEnum.Exit:
                    action = new ToastifyExit();
                    break;

#if DEBUG
                case ToastifyActionEnum.ShowDebugView:
                    action = new ToastifyShowDebugView();
                    break;
#endif

                // TODO: What should we do with these? Are they really necessary?
                case ToastifyActionEnum.CopyTrackInfo:
                case ToastifyActionEnum.SettingsSaved:
                case ToastifyActionEnum.PasteTrackInfo:
                case ToastifyActionEnum.ThumbsUp:
                case ToastifyActionEnum.ThumbsDown:
                default:
                    logger.Warn($"Unhandled toastify action enum value: {actionEnum}");
                    break;
            }

            if (action != null)
                this.actions[actionEnum] = action;
            return action;
        }
    }
}