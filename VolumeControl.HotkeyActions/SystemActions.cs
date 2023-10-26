using System.Diagnostics;
using System.Text;
using System.Windows;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input;
using VolumeControl.Log;

namespace VolumeControl.HotkeyActions
{
    [HotkeyActionGroup("System")]
    public class SystemActions
    {
        private const string SettingName_Target = "FileName";
        private const string SettingDescription_Target = "The absolute or relative path of the target file.";
        private const string SettingName_StartIn = "Working Directory";
        private const string SettingDescription_StartIn = "The working directory to execute the command from.";
        private const string SettingName_Args = "Arguments";
        private const string SettingDescription_Args = "Commandline arguments to run the program with.";
        private const string SettingName_ShowMessageOnError = "Message on Error";
        private const string SettingDescription_ShowMessageOnError = "Shows a messagebox if an error occurs while starting the process.";

        [HotkeyAction(Description = "Runs the specified command.")]
        [HotkeyActionSetting(SettingName_Target, typeof(string), Description = SettingDescription_Target)]
        [HotkeyActionSetting(SettingName_StartIn, typeof(string), Description = SettingDescription_StartIn)]
        [HotkeyActionSetting(SettingName_Args, typeof(string), Description = SettingDescription_Args)]
        [HotkeyActionSetting(SettingName_ShowMessageOnError, typeof(bool), Description = SettingDescription_ShowMessageOnError)]
        public void StartProcess(object sender, HotkeyPressedEventArgs e)
        {
            string target = e.GetValue<string>(SettingName_Target).Trim();

            if (target.Trim().Length == 0)
            {
                return;
            }

            string workingDir = e.GetValue<string>(SettingName_StartIn).Trim();
            string args = e.GetValue<string>(SettingName_Args).Trim();
            bool showMessageOnError = (bool)e.Settings[3].Value!;

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = target,
                    UseShellExecute = true,
                    WorkingDirectory = workingDir,
                    Arguments = args,
                })?.Dispose();
            }
            catch (Exception ex)
            {
                FLog.Error($"StartProcess threw an exception with \"{workingDir}\\{target} {args}\":", ex);
                if (showMessageOnError)
                {
                    var hotkey = (Hotkey)sender;
                    StringBuilder sb = new();
                    sb.AppendLine("Command Failed!");
                    sb.AppendLine($"{SettingName_Target}: {hotkey.GetStringRepresentation()}");

                    MessageBox.Show(
                        sb.ToString(),
                        "StartProcess Error",
                        MessageBoxButton.OK);
                }
            }
        }
    }
}
