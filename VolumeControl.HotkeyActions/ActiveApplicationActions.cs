using System.Runtime.InteropServices;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input;
using VolumeControl.CoreAudio;
using VolumeControl.SDK;
using VolumeControl.SDK.DataTemplates;
using CoreAudio;
using System.Windows.Controls.Primitives;
using VolumeControl.Core;

namespace VolumeControl.HotkeyActions
{
    /// <summary>
    /// Defines actions that affect the current foreground application.
    /// </summary>
    [HotkeyActionGroup("Active Application", GroupColor = "#9F87FF", DefaultDataTemplateProvider = typeof(DataTemplateDictionary))]
    public sealed class ActiveApplicationActions
    {
        #region Fields
        // NOTE: Don't change any names without also changing the translation keys in en-HotkeyActions.json!

        // Select target
        private const string Setting_SelectTarget_Name = "Select Session";
        private const string Setting_SelectTarget_Description = "Selects the session when the action is triggered.";
        // Volume Step
        private const string Setting_VolumeStep_Name = "Volume Step Override";
        private const string Setting_VolumeStep_Description = "Overrides the default volume step for this action.";
        // SetVolume - Volume Level
        private const string Setting_VolumeLevel_Name = "Volume Level";
        private const string Setting_VolumeLevel_Description = "The volume level to set the target session(s) to.";
        // SetVolume - Mute State
        private const string Setting_MuteState_Name = "Mute State";
        private const string Setting_MuteState_Description = "The mute state to set the target session(s) to.";
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


            if ((VCAPI.AudioSessionManager.FindSessionWithPID((uint)pid, includeHiddenSessions: true, includeInactiveSessions: false) ?? VCAPI.AudioSessionManager.FindSessionWithPID((uint)pid, includeHiddenSessions: true, includeInactiveSessions: true)) is AudioSession session)
                return session; //< found with process ID

            return VCAPI.AudioSessionManager.FindSessionWithProcessName(System.Diagnostics.Process.GetProcessById(pid).ProcessName, includeHiddenSessions: true, includeInactiveSessions: false) ?? VCAPI.AudioSessionManager.FindSessionWithProcessName(System.Diagnostics.Process.GetProcessById(pid).ProcessName, includeHiddenSessions: true, includeInactiveSessions: true);
        }
        /// <summary>
        /// Gets all <see cref="AudioSession"/> associated with the current foreground window.
        /// </summary>
        /// <returns>The <see cref="AudioSession"/> associated with the current foreground application, if one was found; otherwise <see langword="null"/>.</returns>
        private static List<AudioSession> GetActiveSessions()
        {
            var hwnd = GetForegroundWindow();

            List<AudioSession> sessions = new();

            if (hwnd == IntPtr.Zero)
                return sessions;

            if (GetWindowThreadProcessId(hwnd, out int pid) == 0)
                return sessions;

            sessions.AddRange(VCAPI.AudioSessionManager.FindSessionsWithPID((uint)pid, DataFlow.Render, includeHiddenSessions: true, includeInactiveSessions: !VCAPI.Settings.HideInactiveSessions));
            if (sessions.Count == 0)
                sessions.AddRange(VCAPI.AudioSessionManager.FindSessionsWithProcessName(System.Diagnostics.Process.GetProcessById(pid).ProcessName, includeHiddenSessions: true, includeInactiveSessions: !VCAPI.Settings.HideInactiveSessions));

            return sessions;
        }
        #endregion Functions

        #region Methods

        #region VolumeUp
        [HotkeyAction(Description = "Increases the volume of the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), "VolumeStepDataTemplate", DefaultValue = 2, Description = Setting_VolumeStep_Description, IsToggleable = true)]
        public void VolumeUp(object? sender, HotkeyPressedEventArgs e)
        {
            var sessions = GetActiveSessions();
            if (sessions.Count > 0)
            {
                var volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

                foreach (var session in sessions)
                {
                    session.Volume += volumeStep;
                }

                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(sessions);
                }
                VCAPI.ShowSessionListNotification(sessions);
            }
        }
        #endregion VolumeUp

        #region VolumeDown
        [HotkeyAction(Description = "Decreases the volume of the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), "VolumeStepDataTemplate", DefaultValue = 2, Description = Setting_VolumeStep_Description, IsToggleable = true)]
        public void VolumeDown(object? sender, HotkeyPressedEventArgs e)
        {
            var sessions = GetActiveSessions();
            if (sessions.Count > 0)
            {
                var volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

                foreach (var session in sessions)
                {
                    session.Volume -= volumeStep;
                }

                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(sessions);
                }

                VCAPI.ShowSessionListNotification(sessions);
            }
        }
        #endregion VolumeDown

        #region SetVolume
        [HotkeyAction(Description = "Sets the volume and/or mute state of the session(s).")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeLevel_Name, typeof(int), "VolumeLevelDataTemplate", DefaultValue = 50, Description = Setting_VolumeLevel_Description, IsToggleable = true, StartsEnabled = true)]
        [HotkeyActionSetting(Setting_MuteState_Name, typeof(bool), Description = Setting_MuteState_Description, IsToggleable = true)]
        public void SetVolume(object? sender, HotkeyPressedEventArgs e)
        {
            var sessions = GetActiveSessions();

            if (sessions.Count > 0)
            {
                var (setVolumeLevel, volumeLevel) = e.GetSetting<int>(Setting_VolumeLevel_Name);
                var (setMuteState, muteState) = e.GetSetting<bool>(Setting_MuteState_Name);

                if (setVolumeLevel || setMuteState)
                {
                    foreach (var session in sessions)
                    {
                        if (setVolumeLevel)
                        {
                            session.Volume = volumeLevel;
                        }
                        if (setMuteState)
                        {
                            session.Mute = muteState;
                        }
                    }
                }

                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(sessions);
                }

                VCAPI.ShowSessionListNotification(sessions);
            }
        }
        #endregion SetVolume

        #region Mute
        [HotkeyAction(Description = "Mutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Mute(object? sender, HotkeyPressedEventArgs e)
        {
            var sessions = GetActiveSessions();
            foreach (var session in sessions)
            {
                session.Mute = true;
            }
            if (sessions.Count > 0)
            {
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(sessions);
                }
                VCAPI.ShowSessionListNotification(sessions);
            }
        }
        #endregion Mute

        #region Unmute
        [HotkeyAction(Description = "Unmutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Unmute(object? sender, HotkeyPressedEventArgs e)
        {
            var sessions = GetActiveSessions();
            foreach (var session in sessions)
            {
                session.Mute = false;
            }
            if (sessions.Count > 0)
            {
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(sessions);
                }
                VCAPI.ShowSessionListNotification(sessions);
            }
        }
        #endregion Unmute

        #region ToggleMute
        [HotkeyAction(Description = "(Un)Mutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void ToggleMute(object? sender, HotkeyPressedEventArgs e)
        {
            var sessions = GetActiveSessions();
            foreach (var session in sessions)
            {
                session.Mute = !session.Mute;
            }
            if (sessions.Count > 0)
            {
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(sessions);
                }
                VCAPI.ShowSessionListNotification(sessions);
            }
        }
        #endregion ToggleMute

        #region Show
        [HotkeyAction(Description = "Shows the active application notification if there are sessions to show.")]
        public void Show(object? sender, HotkeyPressedEventArgs e)
        {
            var sessions = GetActiveSessions();
            if (sessions.Count > 0)
                VCAPI.ShowSessionListNotification(sessions);
        }
        #endregion Show

        #endregion Methods
    }
}
