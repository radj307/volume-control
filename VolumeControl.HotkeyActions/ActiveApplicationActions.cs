using System.Runtime.InteropServices;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input;
using VolumeControl.CoreAudio;
using VolumeControl.SDK;
using VolumeControl.SDK.DataTemplateProviders;

namespace VolumeControl.HotkeyActions
{
    /// <summary>
    /// Defines actions that affect the current foreground application.
    /// </summary>
    [HotkeyActionGroup("Active Application", GroupColor = "#9F87FF")]
    public sealed class ActiveApplicationActions
    {
        #region Fields
        // Select target
        private const string Setting_SelectTarget_Name = "Select Session";
        private const string Setting_SelectTarget_Description = "Selects the session when the action is triggered.";
        // Volume Step
        private const string Setting_VolumeStep_Name = "Volume Step Override";
        private const string Setting_VolumeStep_Description = "Overrides the default volume step for this action.";
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
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), typeof(VolumeStep_NumericUpDown_DataTemplateProvider), DefaultValue = 2, Description = Setting_VolumeStep_Description, IsToggleable = true)]
        public void VolumeUp(object? sender, HotkeyPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                var volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

                session.Volume += volumeStep;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                    VCAPI.ShowSessionListNotification();
                }
            }
        }
        [HotkeyAction(Description = "Decreases the volume of the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), typeof(VolumeStep_NumericUpDown_DataTemplateProvider), DefaultValue = 2, Description = Setting_VolumeStep_Description, IsToggleable = true)]
        public void VolumeDown(object? sender, HotkeyPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                var volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

                session.Volume -= volumeStep;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                    VCAPI.ShowSessionListNotification();
                }
            }
        }
        [HotkeyAction(Description = "Mutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Mute(object? sender, HotkeyPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Mute = true;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                    VCAPI.ShowSessionListNotification();
                }
            }
        }
        [HotkeyAction(Description = "Unmutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Unmute(object? sender, HotkeyPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Mute = false;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                    VCAPI.ShowSessionListNotification();
                }
            }
        }
        [HotkeyAction(Description = "(Un)Mutes the current foreground application.")]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void ToggleMute(object? sender, HotkeyPressedEventArgs e)
        {
            if (GetActiveSession() is AudioSession session)
            {
                session.Mute = !session.Mute;
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                {
                    VCAPI.AudioSessionMultiSelector.SetSelectedSessionsOrCurrentSession(session);
                    VCAPI.ShowSessionListNotification();
                }
            }
        }
        #endregion Methods
    }
}
