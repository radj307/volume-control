namespace VolumeControl.Core
{
    public class SettingsUpgrader
    {
        public SettingsUpgrader()
        {
            Settings.Upgrade();
            Settings.Save();
            Settings.Reload();
        }

        private CoreSettings Settings => CoreSettings.Default;
    }
}
