using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using log4net;
using Microsoft.Win32;

namespace ToastifyAPI.Helpers
{
    public static class System
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(System));

        #region Static Members

        public static string GetOSVersion()
        {
            return GetOperatingSystemManagementObject("Version")?.ToString() ?? Environment.OSVersion.VersionString;
        }

        public static string GetFriendlyOSVersion()
        {
            return GetOperatingSystemManagementObject("Caption")?.ToString() ?? "Unknown";
        }

        private static object GetOperatingSystemManagementObject(string property)
        {
            object @object = null;
            try
            {
                IEnumerable<ManagementObject> managementObjects = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
                                                                 .Get()
                                                                 .OfType<ManagementObject>();
                @object = (from x in managementObjects select x.GetPropertyValue(property)).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.Error($"Error while getting {property}.", e);
            }

            return @object;
        }

        public static string GetNetFramework45PlusVersion()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                var value = key?.GetValue("Release");
                if (value != null)
                    return GetNetFramework45PlusVersionFromRelease((int)value);
            }

            return null;
        }

        private static string GetNetFramework45PlusVersionFromRelease(int releaseKey)
        {
            if (releaseKey >= 461808)
                return "4.7.2 or later";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (releaseKey >= 378389)
                return "4.5";

            return null;
        }

        #endregion
    }
}