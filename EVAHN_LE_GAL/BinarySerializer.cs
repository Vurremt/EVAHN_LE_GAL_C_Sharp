using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace EVAHN_LE_GAL
{
    internal class BinarySerializer : ISerializer
    {
        public void Serialize(string filename, FolderTemp data, string key)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    byte[] hashKey = sha256.ComputeHash(keyBytes);

                    byte[] ivBytes = Encoding.UTF8.GetBytes("ABCDEFGHABCDEFGH"); // 16 characters

                    using (CryptoStream cryptoStream = new CryptoStream(ms, Aes.Create().CreateEncryptor(hashKey, ivBytes), CryptoStreamMode.Write))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(cryptoStream, data);
                    }
                }

                File.WriteAllBytes(filename, ms.ToArray());
            }
        }

        public FolderTemp Deserialize(string filename, string key)
        {
            byte[] fileContent = File.ReadAllBytes(filename);

            using (MemoryStream ms = new MemoryStream(fileContent))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    byte[] hashKey = sha256.ComputeHash(keyBytes);

                    byte[] ivBytes = Encoding.UTF8.GetBytes("ABCDEFGHABCDEFGH"); // 16 characters

                    using (CryptoStream cryptoStream = new CryptoStream(ms, Aes.Create().CreateDecryptor(hashKey, ivBytes), CryptoStreamMode.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        return (FolderTemp)formatter.Deserialize(cryptoStream);
                    }
                }
            }
        }
    }
}
