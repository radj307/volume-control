#if WIN_10

using System.Linq;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace ToastifyAPI
{
    internal static partial class Win32API
    {
        internal static Package FindPackage(string name)
        {
            PackageManager packageManager = new PackageManager();
            var packages = packageManager.FindPackagesForUser(string.Empty);
            return packages.FirstOrDefault(package => package.Id.Name == name);
        }
    }
}

#endif