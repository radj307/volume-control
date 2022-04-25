using System.Configuration;
using VolumeControl.Log;

namespace VolumeControl.Core
{
    public static class SettingsExtensions
    {
        /// <summary>
        /// Set the value of a settings property and write a log message.
        /// </summary>
        /// <param name="appsettings">Application Settings Object Instance.</param>
        /// <param name="propertyName">The name of a property, represented as a string.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>The property was successfully modified.</description></item>
        /// <item><term>false</term><description>The specified property doesn't exist, or is already set to the given value.</description></item>
        /// </list></returns>
        public static bool SetProperty(this ApplicationSettingsBase appsettings, string propertyName, object value)
        {
            if (appsettings[propertyName] != null)
            {
                var copy = appsettings[propertyName];
                if (copy != value)
                {
                    appsettings[propertyName] = value;
                    FLog.Log.Info($"Set '{propertyName}' to '{value}' (was '{copy}')");
                    return true;
                }
            }
            else FLog.Log.Error($"Failed to locate an application property with name '{propertyName}'!");
            return false;
        }
    }
}