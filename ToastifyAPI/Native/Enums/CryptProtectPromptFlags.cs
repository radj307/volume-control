using System;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native.Enums
{
    [Flags]
    public enum CryptProtectPromptFlags
    {
        // prompt on unprotect
        CRYPTPROTECT_PROMPT_ON_UNPROTECT = 0x1,

        // prompt on protect
        CRYPTPROTECT_PROMPT_ON_PROTECT = 0x2
    }
}