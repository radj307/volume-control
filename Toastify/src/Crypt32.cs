using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

// ReSharper disable InconsistentNaming
namespace Toastify
{
    public static class Crypt32
    {
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptProtectData(ref DataBlob pDataIn, string szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, ref CryptProtectPromptStruct pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptProtectData(ref DataBlob pDataIn, string szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptUnprotectData(ref DataBlob pDataIn, StringBuilder szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, ref CryptProtectPromptStruct pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptUnprotectData(ref DataBlob pDataIn, StringBuilder szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DataBlob
        {
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CryptProtectPromptStruct
        {
            public int cbSize;
            public CryptProtectPromptFlags dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;
        }

        [Flags]
        internal enum CryptProtectPromptFlags
        {
            // prompt on unprotect
            CRYPTPROTECT_PROMPT_ON_UNPROTECT = 0x1,

            // prompt on protect
            CRYPTPROTECT_PROMPT_ON_PROTECT = 0x2
        }

        [Flags]
        internal enum CryptProtectFlags
        {
            /// <summary>
            /// For remote-access situations where UI is not an option; if UI was specified on protect or
            /// unprotect operation, the call will fail and GetLastError() will indicate ERROR_PASSWORD_RESTRICTION
            /// </summary>
            CRYPTPROTECT_UI_FORBIDDEN = 0x1,

            /// <summary>
            /// Per machine protected data – any user on machine where CryptProtectData took place may CryptUnprotectData
            /// </summary>
            CRYPTPROTECT_LOCAL_MACHINE = 0x4,

            /// <summary>
            /// Force credential synchronize during CryptProtectData().
            /// Synchronize is the only operation that occurs during this operation.
            /// </summary>
            CRYPTPROTECT_CRED_SYNC = 0x8,

            /// <summary>
            /// Generate an Audit on protect and unprotect operations
            /// </summary>
            CRYPTPROTECT_AUDIT = 0x10,

            /// <summary>
            /// Protect data with a non-recoverable key
            /// </summary>
            CRYPTPROTECT_NO_RECOVERY = 0x20,


            /// <summary>
            /// Verify the protection of a protected blob
            /// </summary>
            CRYPTPROTECT_VERIFY_PROTECTION = 0x40
        }
    }
}