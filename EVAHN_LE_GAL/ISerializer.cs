using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVAHN_LE_GAL
{
    // Interface pour implémenter les deux méthodes de sérialisation et déserialisation
    internal interface ISerializer
    {
        void Serialize(string filename, FolderTemp data, string key);
        FolderTemp Deserialize(string filename, string key);
    }
}