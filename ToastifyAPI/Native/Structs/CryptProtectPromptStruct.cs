using System;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Native.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CryptProtectPromptStruct
    {
        public int cbSize;
        public CryptProtectPromptFlags dwPromptFlags;
        public IntPtr hwndApp;
        public string szPrompt;
    }
}