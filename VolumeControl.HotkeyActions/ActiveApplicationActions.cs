using System.Runtime.InteropServices;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input;
using VolumeControl.CoreAudio;
using VolumeControl.SDK;

namespace VolumeControl.HotkeyActions
{
    /// <summary>
    /// Defines actions that affect the current foreground application.
    /// </summary>
    [HotkeyActionGroup("Active Application", GroupColor = "#9F87FF")]
    public sealed class ActiveApplicationActions
    {
        #region Fields
        private const string Setting_SelectTarget_Name = "Select Session";
        private const string Setting_SelectTarget_Description = "Selects the session when the action is triggered.";
        #endregion Fields

        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;
        #endregion Properties

        #region Functions
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern bool IsWindowEnabled(IntPtr hWnd);
        /// <summary>
        /// Gets the <see cref="AudioSession"/> associated with the current foreground window.
        /// </summary>
        /// <returns>The <see cref="AudioSession"/> associated with the current foreground application, if one was found; otherwise <see langword="null"/>.</returns>
        private static AudioSession? GetActiveSession()
        {
            var hwnd = GetForegroundWindow();

            if (hwnd == IntPtr.Zero)
                return null;

            if (GetWindowThreadProcessId(hwnd, out int pid) == 0)
                return null;


            if (VCAPI.AudioSessionManager.FindSessionWithPID((uint)pid, includeHiddenSessions: true) is AudioSession session)
                return session; //< found with process ID

            return VCAPI.AudioSessionManager.FindSessionWithProcessName(System.Diagnostics.Process.GetProcessById(pid).ProcessName, includeHiddenSessions: true);
        }
        #endregion Functions

        #region Methods
        [HotkeyAction(Description = "Increases the volume of the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Volume += VCAPI.Settings.VolumeStepSize;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                }
            }
        }
        [HotkeyAction(Description = "Decreases the volume of the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Volume -= VCAPI.Settings.VolumeStepSize;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                }
            }
        }
        [HotkeyAction(Description = "Mutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Mute = true;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                }
            }
        }
        [HotkeyAction(Description = "Unmutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Mute = false;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                }
            }
        }
        [HotkeyAction(Description = "(Un)Mutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Mute = !session.Mute;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                }
            }
        }
        #endregion Methods
    }
}
