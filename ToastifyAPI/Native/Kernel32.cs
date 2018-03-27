using System;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.Delegates;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Native
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(int dwErrorCode);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ExecutionStateFlags SetThreadExecutionState(ExecutionStateFlags esFlags);

        /// <summary>
        /// Enumerates resources of a specified type within a binary module.
        /// </summary>
        /// <param name="hModule">
        ///   A handle to a module to be searched.
        ///   If this parameter is NULL, that is equivalent to passing in a handle to the module used to create the current process.
        /// </param>
        /// <param name="lpszType"> The type of the resource for which the name is being enumerated. </param>
        /// <param name="lpEnumFunc"> The callback function to be called for each enumerated resource name. </param>
        /// <param name="lParam"> An application-defined value passed to the callback function. This parameter can be used in error checking. </param>
        /// <returns> The return value is TRUE if the function succeeds or FALSE if the function does not find a resource of the type specified, or if the function fails for another reason. </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumResourceNames(IntPtr hModule, string lpszType, EnumResNameProcDelegate lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// Enumerates resources of a specified type within a binary module.
        /// </summary>
        /// <param name="hModule">
        ///   A handle to a module to be searched.
        ///   If this parameter is NULL, that is equivalent to passing in a handle to the module used to create the current process.
        /// </param>
        /// <param name="dwID"> The ID of the resource. </param>
        /// <param name="lpEnumFunc"> The callback function to be called for each enumerated resource ID. </param>
        /// <param name="lParam"> An application-defined value passed to the callback function. This parameter can be used in error checking. </param>
        /// <returns> The return value is TRUE if the function succeeds or FALSE if the function does not find a resource of the type specified, or if the function fails for another reason. </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumResourceNames(IntPtr hModule, ResourceType dwID, EnumResNameProcDelegate lpEnumFunc, IntPtr lParam);
    }
}