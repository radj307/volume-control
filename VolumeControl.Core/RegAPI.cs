using Microsoft.Win32;

namespace VolumeControl.Core
{
    public static class RegAPI
    {
        public static readonly string KEY_RUN = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static readonly string SUBKEY_NAME = "VolumeControl";

        public static void EnableRunOnStartup()
        {
            RegistryKey? runkey = RunKey;
            if (runkey != null)
            {
                runkey.SetValue(SUBKEY_NAME, Application.ExecutablePath.ToString());
            }
            else throw new Exception($"Run-On-Startup Couldn't be Enabled!\n\tCannot create subkey entry for \"{SUBKEY_NAME}\" in \"{KEY_RUN}\"!\n\tTry running the program with administrator permissions to enable this setting.");
        }
        public static bool DisableRunOnStartup()
        {
            if (RunKey != null)
            {
                var val = RunKey?.GetValue(SUBKEY_NAME, null);
                if (val == null)
                    return false;
                RunKey?.DeleteValue(SUBKEY_NAME, false);
                return true;
            }
            return false;
        }
        public static RegistryKey? RunKey
        {
            get => Registry.CurrentUser.OpenSubKey(KEY_RUN, true);
            set => value?.DeleteValue(SUBKEY_NAME);
        }
    }
}
