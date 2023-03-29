using System.Runtime.InteropServices;
using VolumeControl.Audio;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Actions;
using VolumeControl.SDK;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Defines actions that affect the current foreground application.
    /// </summary>
    [HotkeyActionGroup("Active Application", GroupColor = "#9F87FF")]
    public class ActiveApplicationActions
    {
        #region Properties
        private static AudioAPI AudioAPI => VCAPI.Default.AudioAPI;
        #endregion Properties

        #region Functions
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);
        private static ISession? GetActiveSession()
        {
            var hwnd = GetForegroundWindow();

            if (hwnd == IntPtr.Zero)
                return null;

            _ = GetWindowThreadProcessId((int)hwnd, out int pid);

            if (pid == 0)
                return null;

            return AudioAPI.FindSessionWithID(pid);
        }
        #endregion Functions

        #region Methods
        [HotkeyAction(Description = "Increases the volume of the current foreground application.")]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is ISession session)
            {
                session.Volume += AudioAPI.VolumeStepSize;
            }
        }
        [HotkeyAction(Description = "Decreases the volume of the current foreground application.")]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is ISession session)
            {
                session.Volume -= AudioAPI.VolumeStepSize;
            }
        }
        [HotkeyAction(Description = "Mutes the current foreground application.")]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is ISession session)
            {
                session.Muted = true;
            }
        }
        [HotkeyAction(Description = "Unmutes the current foreground application.")]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is ISession session)
            {
                session.Muted = false;
            }
        }
        [HotkeyAction(Description = "(Un)Mutes the current foreground application.")]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is ISession session)
            {
                session.Muted = !session.Muted;
            }
        }
        #endregion Methods
    }
}
