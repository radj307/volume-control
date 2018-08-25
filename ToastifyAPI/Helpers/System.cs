using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using log4net;

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

        #endregion
    }
}