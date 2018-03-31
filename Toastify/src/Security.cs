using log4net;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;

namespace Toastify
{
    public static class Security
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Security));

        public static bool ProtectedDataExists(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(@"File name is not valid", nameof(fileName));

            string secFilePath = Path.Combine(App.LocalApplicationData, fileName);
            return File.Exists(secFilePath);
        }

        #region Proxy Password

        internal static SecureString GetSecureProxyPassword()
        {
            SecureString secureString = GetProtectedSecureString("proxy.sec");
            return secureString != null && secureString.Length > 0 ? secureString : null;
        }

        internal static void SaveProxyPassword(SecureString secureString)
        {
            SaveProtectedData(secureString, "proxy.sec");
        }

        #endregion Proxy Password

        internal static byte[] GetProtectedData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(@"File name is not valid", nameof(fileName));

            byte[] encryptedData = GetProtectedDataInternal(fileName, out byte[] entropy);
            return ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
        }

        internal static SecureString GetProtectedSecureString(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(@"File name is not valid", nameof(fileName));

            byte[] encryptedData = GetProtectedDataInternal(fileName, out byte[] entropy);
            if (encryptedData == null || encryptedData.Length == 0)
                return null;

            IntPtr dataPtr = Marshal.AllocHGlobal(encryptedData.Length);
            Marshal.Copy(encryptedData, 0, dataPtr, encryptedData.Length);

            IntPtr entropyPtr = Marshal.AllocHGlobal(entropy.Length);
            Marshal.Copy(entropy, 0, entropyPtr, entropy.Length);

            try
            {
                DataBlob dataBlob = new DataBlob
                {
                    cbData = encryptedData.Length,
                    pbData = dataPtr
                };
                DataBlob entropyBlob = new DataBlob
                {
                    cbData = entropy.Length,
                    pbData = entropyPtr
                };
                DataBlob outBlob = new DataBlob();

                // Crypt
                bool success = Crypt32.CryptUnprotectData(ref dataBlob, null, ref entropyBlob, IntPtr.Zero, IntPtr.Zero, CryptProtectFlags.CRYPTPROTECT_LOCAL_MACHINE, ref outBlob);
                if (!success)
                {
                    int error = Marshal.GetLastWin32Error();
                    logger.Error($"CryptUnprotectData failed with error code {error}!");
                }

                // Save to file
                byte[] bytes = null;
                char[] chars = null;
                try
                {
                    if (outBlob.cbData <= 0)
                        return null;

                    SecureString secureString = new SecureString();

                    bytes = new byte[outBlob.cbData];
                    Marshal.Copy(outBlob.pbData, bytes, 0, bytes.Length);

                    chars = Encoding.Unicode.GetChars(bytes);
                    foreach (char c in chars)
                        secureString.AppendChar(c);

                    return secureString;
                }
                finally
                {
                    Marshal.FreeHGlobal(outBlob.pbData);

                    try
                    {
                        unsafe
                        {
                            // Zero out the byte array
                            if (bytes != null)
                            {
                                byte[] zeroB = new byte[bytes.Length];
                                fixed (byte* pb = &bytes[0])
                                {
                                    Marshal.Copy(zeroB, 0, new IntPtr(pb), zeroB.Length);
                                }
                            }

                            // Zero out the char array
                            if (chars != null)
                            {
                                char[] zeroC = new char[chars.Length];
                                fixed (char* pc = &chars[0])
                                {
                                    Marshal.Copy(zeroC, 0, new IntPtr(pc), zeroC.Length);
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (bytes != null)
                            Array.Clear(bytes, 0, bytes.Length);
                        if (chars != null)
                            Array.Clear(chars, 0, bytes.Length);
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(entropyPtr);
                Marshal.FreeHGlobal(dataPtr);
            }
        }

        internal static void SaveProtectedData(byte[] plaintext, string fileName)
        {
            if (plaintext == null)
            {
                logger.Warn($"{nameof(plaintext)} is null");
                return;
            }
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(@"File name is not valid", nameof(fileName));

            // Generate entropy
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

            // Write to file
            SaveProtectedDataInternal(ciphertext, entropy, fileName);
        }

        internal static void SaveProtectedData(SecureString secureString, string fileName)
        {
            if (secureString == null)
            {
                logger.Warn($"{nameof(secureString)} is null");
                return;
            }
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(@"File name is not valid", nameof(fileName));

            // Generate entropy
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            IntPtr entropyPtr = Marshal.AllocHGlobal(entropy.Length);
            Marshal.Copy(entropy, 0, entropyPtr, entropy.Length);

            // Get a BSTR from the SecureString
            // NOTE: A BSTR is a Unicode (UTF-16) string, which could contain multiple NULL characters
            IntPtr unmanagedString = Marshal.SecureStringToBSTR(secureString);

            try
            {
                DataBlob dataBlob = new DataBlob
                {
                    cbData = secureString.Length * 2,
                    pbData = unmanagedString
                };
                DataBlob entropyBlob = new DataBlob
                {
                    cbData = entropy.Length,
                    pbData = entropyPtr
                };
                DataBlob outBlob = new DataBlob();

                // Crypt
                bool success = Crypt32.CryptProtectData(ref dataBlob, null, ref entropyBlob, IntPtr.Zero, IntPtr.Zero, CryptProtectFlags.CRYPTPROTECT_LOCAL_MACHINE, ref outBlob);
                if (!success)
                {
                    int error = Marshal.GetLastWin32Error();
                    logger.Error($"CryptProtectData failed with error code {error}!");
                }

                // Save to file
                try
                {
                    byte[] cryptedData = new byte[outBlob.cbData];
                    Marshal.Copy(outBlob.pbData, cryptedData, 0, cryptedData.Length);

                    SaveProtectedDataInternal(cryptedData, entropy, fileName);
                }
                finally
                {
                    Marshal.FreeHGlobal(outBlob.pbData);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(entropyPtr);
                Marshal.ZeroFreeBSTR(unmanagedString);
            }
        }

        #region Internal

        private static byte[] GetProtectedDataInternal(string fileName, out byte[] entropy)
        {
            string secFilePath = Path.Combine(App.LocalApplicationData, fileName);
            if (!File.Exists(secFilePath))
            {
                entropy = null;
                return null;
            }

            ProtectedText pt;
            using (FileStream fs = new FileStream(secFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                pt = (ProtectedText)binaryFormatter.Deserialize(fs);
            }

            entropy = pt.entropy;
            return pt.ciphertext;
        }

        private static void SaveProtectedDataInternal(byte[] cryptedData, byte[] entropy, string fileName)
        {
            string secFilePath = Path.Combine(App.LocalApplicationData, fileName);

            // Write to file
            using (FileStream fs = new FileStream(secFilePath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fs, new ProtectedText { entropy = entropy, ciphertext = cryptedData });
            }

            // Only allow access to the file to the current user
            var acl = File.GetAccessControl(secFilePath);
            acl.AddAccessRule(new FileSystemAccessRule(
                WindowsIdentity.GetCurrent().Name,
                FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.Delete,
                InheritanceFlags.None,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow));
            acl.SetAccessRuleProtection(true, false);
            File.SetAccessControl(secFilePath, acl);
        }

        #endregion Internal

        [Serializable]
        private class ProtectedText
        {
            public byte[] entropy;
            public byte[] ciphertext;
        }
    }
}