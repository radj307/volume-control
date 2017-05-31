using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AutoHotkey.Interop
{
    internal static class Util
    {
        public static string FindEmbededResourceName(Assembly assembly, string path)
        {
            path = Regex.Replace(path, @"[/\\]", ".");

            if (!path.StartsWith("."))
                path = "." + path;

            var names = assembly.GetManifestResourceNames();

            return names.FirstOrDefault(name => name.EndsWith(path));
        }

        public static void ExtractEmbeddedResourceToFile(Assembly assembly, string embededResourcePath, string targetFileName)
        {
            //ensure directory exists
            FileInfo fi = new FileInfo(targetFileName);
            if (!fi.Exists)
                return;

            var dir = Path.GetDirectoryName(targetFileName);
            if (dir != null)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                if (!di.Exists)
                    di.Create();

                using (var readStream = assembly.GetManifestResourceStream(embededResourcePath))
                {
                    using (var writeStream = File.Open(targetFileName, FileMode.Create))
                    {
                        readStream?.CopyTo(writeStream);
                        readStream?.Flush();
                    }
                }
            }
        }

        public static bool Is64Bit()
        {
            return IntPtr.Size == 8;
        }

        public static bool Is32Bit()
        {
            return IntPtr.Size == 4;
        }



        public static void EnsureAutoHotkeyLoaded()
        {
            if (dllHandle.IsValueCreated)
                return;

            var dummy = dllHandle.Value;
        }

        private static readonly Lazy<SafeLibraryHandle> dllHandle = new Lazy<SafeLibraryHandle>(LoadAutoHotKeyDll);

        private static SafeLibraryHandle LoadAutoHotKeyDll()
        {
            //Locate and Load 32bit or 64bit version of AutoHotkey.dll
            string tempFolderPath = Path.Combine(Path.GetTempPath(), "AutoHotkey.Interop");
            const string path32 = @"x86\AutoHotkey.dll";
            const string path64 = @"x64\AutoHotkey.dll";

            var loadDllFromFileOrResource = new Func<string, SafeLibraryHandle>(relativePath =>
            {
                if (File.Exists(relativePath))
                    return SafeLibraryHandle.LoadLibrary(relativePath);

                var assembly = typeof(AutoHotkeyEngine).Assembly;
                var resource = FindEmbededResourceName(assembly, relativePath);

                if (resource != null)
                {
                    var target = Path.Combine(tempFolderPath, relativePath);
                    ExtractEmbeddedResourceToFile(assembly, resource, target);
                    return SafeLibraryHandle.LoadLibrary(target);
                }

                return null;
            });

            return Is32Bit()
                ? loadDllFromFileOrResource(path32)
                : Is64Bit()
                    ? loadDllFromFileOrResource(path64)
                    : null;
        }
    }
}