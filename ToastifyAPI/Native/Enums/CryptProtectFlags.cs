using System;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native.Enums
{
    [Flags]
    public enum CryptProtectFlags
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