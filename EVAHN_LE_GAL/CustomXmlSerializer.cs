using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EVAHN_LE_GAL
{
    // CustomXmlSerializer implémente l'interface ISerializer pour qu'on puisse faire facilement un Factory et garder une certaine forme d'encapsulation et ne pas repeter de code quand on l'appelera
    internal class CustomXmlSerializer : ISerializer
    {
        public void Serialize(string filename, FolderTemp data, string key)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Hashe la clé et le iv dans le bon format
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    byte[] hashKey = sha256.ComputeHash(keyBytes);

                    byte[] ivBytes = Encoding.UTF8.GetBytes("ABCDEFGHABCDEFGH"); // 16 characters

                    // Creation du flux crypté et écriture
                    using (CryptoStream cryptoStream = new CryptoStream(ms, Aes.Create().CreateEncryptor(hashKey, ivBytes), CryptoStreamMode.Write))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(FolderTemp));
                        ser.Serialize(cryptoStream, data);
                    }
                }

                // Sauvegarde du flux crypté dans le fichier
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
                    // Hashe la clé et le iv dans le bon format
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    byte[] hashKey = sha256.ComputeHash(keyBytes);

                    byte[] ivBytes = Encoding.UTF8.GetBytes("ABCDEFGHABCDEFGH"); // 16 characters

                    // Creation du flux crypté et lecture puis decryptage
                    using (CryptoStream cryptoStream = new CryptoStream(ms, Aes.Create().CreateDecryptor(hashKey, ivBytes), CryptoStreamMode.Read))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(FolderTemp));
                        return (FolderTemp)ser.Deserialize(cryptoStream); // On retourne l'élément créer, si null alors la clé n'était pas bonne (ou élément vide, dans tous les cas = problèmes)
                    }
                }
            }
        }
    }
}
