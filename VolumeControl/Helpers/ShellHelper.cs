using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using VolumeControl.Log;

namespace VolumeControl.Helpers
{
    public static class ShellHelper
    {
        #region OpenIn
        /// <summary>
        /// Starts an application with <paramref name="arguments"/> in the specified <paramref name="workingDirectory"/> as a new process.
        /// </summary>
        /// <param name="appPath">The path of the application to start.</param>
        /// <param name="workingDirectory">The working directory to open the specified application in.</param>
        /// <param name="arguments">Commandline arguments for the application.</param>
        /// <returns><see langword="true"/> when the process was started successfully; otherwise, <see langword="false"/>.</returns>
        public static bool OpenIn(string appPath, string workingDirectory, string arguments)
        {
            using var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(appPath)
                {
                    WorkingDirectory = workingDirectory,
                    Arguments = arguments,
                }
            };
            return proc.Start();
        }
        /// <summary>
        /// Starts an application in the specified <paramref name="workingDirectory"/> as a new process.
        /// </summary>
        /// <inheritdoc cref="OpenIn(string, string, string)"/>
        public static bool OpenIn(string appPath, string workingDirectory)
            => OpenIn(appPath, workingDirectory, string.Empty);
        #endregion OpenIn

        #region Open
        /// <summary>
        /// Starts an application with <paramref name="arguments"/> in the current directory.
        /// </summary>
        /// <param name="appPath">The path of the application to start.</param>
        /// <param name="arguments">Commandline arguments for the application.</param>
        public static bool Open(string appPath, string arguments)
            => OpenIn(appPath, Environment.CurrentDirectory, arguments);
        /// <summary>
        /// Starts an application in the current directory.
        /// </summary>
        /// <param name="appPath">The path of the application to start.</param>
        public static bool Open(string appPath)
            => OpenIn(appPath, Environment.CurrentDirectory, string.Empty);
        #endregion Open

        #region OpenWith
        /// <summary>
        /// Opens the specified <paramref name="filePath"/> with an application in the specified <paramref name="appBasePath"/> directory.
        /// </summary>
        /// <param name="filePath">The path of the file to open.</param>
        /// <param name="appBasePath">The directory where the application to use to open the file is located.</param>
        /// <param name="args">Commandline arguments for the application.</param>
        /// <returns><see langword="true"/> when the process was started successfully; otherwise, <see langword="false"/>.</returns>
        public static bool OpenWith(string filePath, string appBasePath, string args)
        {
            using var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true,
                    WorkingDirectory = appBasePath,
                    Arguments = args,
                }
            };
            return proc.Start();
        }
        /// <inheritdoc cref="OpenWith(string, string, string)"/>
        public static bool OpenWith(string filePath, string appBasePath)
            => OpenWith(filePath, appBasePath, string.Empty);
        #endregion OpenWith

        #region OpenWithDefault
        /// <summary>
        /// Opens the specified <paramref name="filePath"/> with the default application for that filetype.
        /// </summary>
        /// <param name="filePath">The path of the file to open.</param>
        /// <returns><see langword="true"/> when the process was started successfully; otherwise, <see langword="false"/>.</returns>
        public static bool OpenWithDefault(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            using var proc = new Process() { StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true } };
            return proc.Start();
        }
        #endregion OpenWithDefault

        #region Start
        /// <summary>
        /// Starts a new process using the specified <paramref name="processStartInfo"/> object.
        /// </summary>
        /// <param name="processStartInfo">A <see cref="ProcessStartInfo"/> object that defines the process to start.</param>
        /// <returns><see langword="true"/> when the process was started successfully; otherwise, <see langword="false"/>.</returns>
        public static bool Start(ProcessStartInfo processStartInfo)
        {
            using var proc = new Process() { StartInfo = processStartInfo };
            return proc.Start();
        }
        #endregion Start

        #region OpenFolderAndSelectItem

        #region P/Invoke
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);
        #endregion P/Invoke

        /// <summary>
        /// Opens the file explorer with the specified <paramref name="filePath"/> selected.
        /// </summary>
        /// <param name="filePath">A file to select in the file explorer.</param>
        public static void OpenFolderAndSelectItem(string filePath)
        {
            if (Path.GetDirectoryName(filePath) is not string directoryPath)
            {
                FLog.Error($"Cannot get directory from path '{filePath}'!");
                return;
            }

            SHParseDisplayName(directoryPath, IntPtr.Zero, out IntPtr nativeFolder, 0, out uint psfgaoOut);

            if (nativeFolder == IntPtr.Zero)
            {
                FLog.Error($"Cannot locate directory '{directoryPath}'!");
                return;
            }

            SHParseDisplayName(filePath, IntPtr.Zero, out IntPtr nativeFile, 0, out psfgaoOut);

            IntPtr[] fileArray;
            if (nativeFile == IntPtr.Zero)
            {
                // Open the folder without the file selected if we can't find the file
                fileArray = Array.Empty<IntPtr>();
            }
            else
            {
                fileArray = new IntPtr[] { nativeFile };
            }

            FLog.Debug($"Opening and selecting '{filePath}' in the file explorer.");

            _ = SHOpenFolderAndSelectItems(nativeFolder, (uint)fileArray.Length, fileArray, 0);

            Marshal.FreeCoTaskMem(nativeFolder);
            if (nativeFile != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(nativeFile);
            }
        }
        #endregion OpenFolderAndSelectItem

        #region CanWriteToDirectory
        /// <summary>
        /// Checks if the application has write access to the specified <paramref name="directoryPath"/>.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to check.</param>
        /// <returns><see langword="true"/> when the application can write to <paramref name="directoryPath"/>; otherwise, <see langword="false"/>.</returns>
        public static bool CanWriteToDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath)) return false;
            const FileSystemRights writeDataPermission = FileSystemRights.WriteData;

            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                bool identityHasGroups = identity.Groups != null;

                DirectoryInfo di = new DirectoryInfo(directoryPath);
                DirectorySecurity acl = di.GetAccessControl(AccessControlSections.All);
                AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

                //Go through the rules returned from the DirectorySecurity
                foreach (AuthorizationRule rule in rules)
                {
                    var ruleIdentityRef = rule.IdentityReference;
                    if (ruleIdentityRef == identity.Owner || (identityHasGroups && identity.Groups!.Contains(ruleIdentityRef)))
                    {
                        var accessRule = (FileSystemAccessRule)rule;

                        if (accessRule.AccessControlType == AccessControlType.Allow
                            && accessRule.FileSystemRights.HasFlag(writeDataPermission))
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
        #endregion CanWriteToDirectory
    }
}
