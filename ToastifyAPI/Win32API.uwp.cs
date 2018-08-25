#if WIN_10

using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace ToastifyAPI
{
    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class Win32API
    {
        #region Static Members

        internal static Package FindPackage(string name)
        {
            var packageManager = new PackageManager();
            IEnumerable<Package> packages = packageManager.FindPackagesForUser(string.Empty);
            return packages.FirstOrDefault(package => package.Id.Name == name);
        }

        #endregion
    }
}

#endif