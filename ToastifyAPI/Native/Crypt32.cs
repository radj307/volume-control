using System;
using System.Runtime.InteropServices;
using System.Text;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native
{
    public static class Crypt32
    {
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptProtectData(ref DataBlob pDataIn, string szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, ref CryptProtectPromptStruct pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptProtectData(ref DataBlob pDataIn, string szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptUnprotectData(ref DataBlob pDataIn, StringBuilder szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, ref CryptProtectPromptStruct pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptUnprotectData(ref DataBlob pDataIn, StringBuilder szDataDescr, ref DataBlob pOptionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, CryptProtectFlags dwFlags, ref DataBlob pDataOut);
    }
}