using System.Runtime.InteropServices;

namespace VolumeControl
{
    public enum VirtualKeyCode : byte
    {
        None = 0x00,
        VK_VOLUME_MUTE = 0xAD,
        VK_VOLUME_DOWN = 0xAE,
        VK_VOLUME_UP = 0xAF,
        VK_MEDIA_NEXT_TRACK = 0xB0,
        VK_MEDIA_PREV_TRACK = 0xB1,
        VK_MEDIA_STOP = 0xB2,
        VK_MEDIA_PLAY_PAUSE = 0xB3,
    }
    public static class Keyboard
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void Event(VirtualKeyCode virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        public static void SendMediaKey(ToastifyActionEnum action)
        {
            VirtualKeyCode virtualKey;
            switch (action)
            {
            case ToastifyActionEnum.Stop:
                virtualKey = VirtualKeyCode.VK_MEDIA_STOP;
                break;

            case ToastifyActionEnum.PlayPause:
                virtualKey = VirtualKeyCode.VK_MEDIA_PLAY_PAUSE;
                break;

            case ToastifyActionEnum.Mute:
                virtualKey = VirtualKeyCode.VK_VOLUME_MUTE;
                break;

            case ToastifyActionEnum.VolumeDown:
                virtualKey = VirtualKeyCode.VK_VOLUME_DOWN;
                break;

            case ToastifyActionEnum.VolumeUp:
                virtualKey = VirtualKeyCode.VK_VOLUME_UP;
                break;

            case ToastifyActionEnum.PreviousTrack:
                virtualKey = VirtualKeyCode.VK_MEDIA_PREV_TRACK;
                break;

            case ToastifyActionEnum.NextTrack:
                virtualKey = VirtualKeyCode.VK_MEDIA_NEXT_TRACK;
                break;

            case ToastifyActionEnum.None:
            case ToastifyActionEnum.FastForward:
            case ToastifyActionEnum.Rewind:
            case ToastifyActionEnum.ShowToast:
            case ToastifyActionEnum.ShowSpotify:
            case ToastifyActionEnum.CopyTrackInfo:
            case ToastifyActionEnum.SettingsSaved:
            case ToastifyActionEnum.PasteTrackInfo:
            case ToastifyActionEnum.ThumbsUp:
            case ToastifyActionEnum.ThumbsDown:
            case ToastifyActionEnum.Exit:
#if DEBUG
            case ToastifyActionEnum.ShowDebugView:
#endif
            default:
                return;
            }

            Event(virtualKey, 0, 1, IntPtr.Zero);
        }
    }
}
