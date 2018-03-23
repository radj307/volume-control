using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Toastify
{
    public static class Security
    {
        public static bool ProtectedDataExists(string fileName)
        {
            string secFilePath = Path.Combine(App.LocalApplicationData, fileName);
            return File.Exists(secFilePath);
        }

        internal static string GetProtectedProxyPassword()
        {
            byte[] data = GetProtectedData("proxy.sec");
            return data != null ? Encoding.UTF8.GetString(data) : null;
        }

        internal static void SaveProtectedProxyPassword(byte[] plaintext)
        {
            SaveProtectedData(plaintext, "proxy.sec");
        }

        internal static byte[] GetProtectedData(string fileName)
        {
            string secFilePath = Path.Combine(App.LocalApplicationData, fileName);
            if (!File.Exists(secFilePath))
                return null;

            ProtectedText pt;
            using (FileStream fs = new FileStream(secFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                pt = (ProtectedText)binaryFormatter.Deserialize(fs);
            }

            return ProtectedData.Unprotect(pt.ciphertext, pt.entropy, DataProtectionScope.CurrentUser);
        }

        internal static void SaveProtectedData(byte[] plaintext, string fileName)
        {
            string secFilePath = Path.Combine(App.LocalApplicationData, fileName);

            // Generate entropy
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

            // Write to file
            using (FileStream fs = new FileStream(secFilePath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fs, new ProtectedText { entropy = entropy, ciphertext = ciphertext });
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

        [Serializable]
        private class ProtectedText
        {
            public byte[] entropy;
            public byte[] ciphertext;
        }
    }
}