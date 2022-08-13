using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extension methods for the <see cref="Process"/> class.
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>Opens an existing local process object.</summary>
        /// <param name="dwDesiredAccess">The access to the process object. This access right is checked against the security descriptor for the process. This parameter can be one or more of the process access rights.If the caller has enabled the SeDebugPrivilege privilege, the requested access is granted regardless of the contents of the security descriptor.</param>
        /// <param name="bInheritHandle">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="dwProcessId">The identifier of the local process to be opened.If the specified process is the System Idle Process(0x00000000), the function fails and the last error code is ERROR_INVALID_PARAMETER.If the specified process is the System process or one of the Client Server Run-Time Subsystem(CSRSS) processes, this function fails and the last error code is ERROR_ACCESS_DENIED because their access restrictions prevent user-level code from opening them.If you are using GetCurrentProcessId as an argument to this function, consider using GetCurrentProcess instead of OpenProcess, for improved performance.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process. If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
        [DllImport("Kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        private const int PROCESS_QUERY_LIMITED_INFORMATION = 4096;
        private const int SYNCHRONIZE = 1048576;

        /// <summary>
        /// Retrieves timing information for the specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process whose timing information is sought. The handle must have the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right. For more information, see Process Security and Access Rights.<br/>Windows Server 2003 and Windows XP:  The handle must have the PROCESS_QUERY_INFORMATION access right.</param>
        /// <param name="lpCreationTime">A pointer to a FILETIME structure that receives the creation time of the process.</param>
        /// <param name="lpExitTime">A pointer to a FILETIME structure that receives the exit time of the process. If the process has not exited, the content of this structure is undefined.</param>
        /// <param name="lpKernelTime">A pointer to a FILETIME structure that receives the amount of time that the process has executed in kernel mode. The time that each of the threads of the process has executed in kernel mode is determined, and then all of those times are summed together to obtain this value.</param>
        /// <param name="lpUserTime">A pointer to a FILETIME structure that receives the amount of time that the process has executed in user mode. The time that each of the threads of the process has executed in user mode is determined, and then all of those times are summed together to obtain this value. Note that this value can exceed the amount of real time elapsed (between lpCreationTime and lpExitTime) if the process executes across multiple CPU cores.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br/>If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("Kernel32.dll")]
        private static extern int GetProcessTimes(IntPtr hProcess, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);
        /// <inheritdoc cref="GetProcessTimes(IntPtr, out long, out long, out long, out long)"/>
        /// <param name="hProcess">A process handle.</param>
        /// <param name="creationTime">A pointer to a FILETIME structure that receives the creation time of the process.</param>
        /// <param name="exitTime">A pointer to a FILETIME structure that receives the exit time of the process. If the process has not exited, the content of this structure is undefined.</param>
        /// <param name="kernelTime">A pointer to a FILETIME structure that receives the amount of time that the process has executed in kernel mode. The time that each of the threads of the process has executed in kernel mode is determined, and then all of those times are summed together to obtain this value.</param>
        /// <param name="userTime">A pointer to a FILETIME structure that receives the amount of time that the process has executed in user mode. The time that each of the threads of the process has executed in user mode is determined, and then all of those times are summed together to obtain this value. Note that this value can exceed the amount of real time elapsed (between <paramref name="creationTime"/> and <paramref name="exitTime"/>) if the process executes across multiple CPU cores.</param>
        private static void GetProcessTimes(IntPtr hProcess, out DateTime? creationTime, out DateTime? exitTime, out DateTime? kernelTime, out DateTime? userTime)
        {
            if (GetProcessTimes(hProcess, out long creation, out long exit, out long kernel, out long user) == 0)
            {
                creationTime = null;
                exitTime = null;
                kernelTime = null;
                userTime = null;
                return;
            }
            creationTime = DateTime.FromFileTime(creation);
            exitTime = DateTime.FromFileTime(exit);
            kernelTime = DateTime.FromFileTime(kernel);
            userTime = DateTime.FromFileTime(user);
        }

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <returns>If the function succeeds, the return value is nonzero.<br/>If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// This fixes a bug that microsoft hasn't fixed since at least Windows Vista that occurs when calling <see cref="Process.HasExited"/>.<br/>
        /// <b>DO NOT USE THE <see cref="Process.HasExited"/> PROPERTY, USE THIS INSTEAD!</b>
        /// </summary>
        /// <remarks>Sources:<br/>
        /// https://www.giorgi.dev/net/access-denied-process-bugs/ <br/>
        /// https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess <br/>
        /// https://docs.microsoft.com/en-ca/windows/win32/api/processthreadsapi/nf-processthreadsapi-getprocesstimes <br/>
        /// https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle <br/>
        /// </remarks>
        /// <param name="p">Process instance.</param>
        /// <returns>True if the process has exited, false if the process is still running.</returns>
        public static bool HasExited(this Process p)
        {
            if (p.Id == 0)
                return false;
            IntPtr handle = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION | SYNCHRONIZE, 0, p.Id);
            if (handle == IntPtr.Zero)
                return false;
            GetProcessTimes(handle, out DateTime? _, out DateTime? exitTime, out DateTime? _, out DateTime? _);
            _ = CloseHandle(handle);
            return exitTime != null && DateTime.Now < exitTime;
        }

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        /// <summary>
        /// Gets the full path to the specified <see cref="Process"/>, <paramref name="p"/>.
        /// </summary>
        /// <param name="p"><see cref="Process"/></param>
        /// <returns>The filepath of <paramref name="p"/> if successful; otherwise <see langword="null"/>.</returns>
        public static string? GetMainModulePath(this Process p)
        {
            IntPtr hProc = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, 0, p.Id);

            if (hProc == IntPtr.Zero)
                return null;

            const int bufLen = 261;

            var sbuf = new StringBuilder(bufLen);

            string? result = null;

            if (GetModuleFileNameEx(hProc, IntPtr.Zero, sbuf, bufLen) > 0)
                result = sbuf.ToString();

            return result;
        }

        /// <summary>
        /// Gets the full path to the specified <see cref="Process"/>, <paramref name="p"/>.<br/>
        /// <b>Note that this method uses WMI to make an SQL query, and as such, is very slow!</b>
        /// </summary>
        /// <param name="p"><see cref="Process"/></param>
        /// <returns>The filepath of <paramref name="p"/> if successful; otherwise <see langword="null"/>.</returns>
        public static string? GetMainModulePathWMI(this Process p)
        {
            using var searcher = new ManagementObjectSearcher($"SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = {p.Id}");
            using ManagementObjectCollection? results = searcher.Get();
            return results.Cast<ManagementObject>().FirstOrDefault() is ManagementObject mo ? (string)mo["ExecutablePath"] : null;
        }
    }
}
