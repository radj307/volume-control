using System.Management;

namespace AudioAPI
{
    public class System
    {
        public static bool VersionGreaterThan(int major, int minor)
        {
            int os_major = Environment.OSVersion.Version.Major, os_minor = Environment.OSVersion.Version.Minor;
            return os_major > major || (os_major == major && os_minor >= minor);
        }
        public static object? GetManagementObject(string property)
        {
            object? obj = null;
            try
            {
                IEnumerable<ManagementObject> objects = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>();
                obj = (from it in objects select it.GetPropertyValue(property)).FirstOrDefault();
            }
            catch (Exception) {}
            return obj;
        }
    }
}
