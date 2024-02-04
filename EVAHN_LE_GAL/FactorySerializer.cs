using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EVAHN_LE_GAL
{
    public enum typeSerializer
    {
        Binary,
        Xml
    }

    internal static class FactorySerializer
    {
        public static ISerializer GetSerializer(typeSerializer typeS)
        {
            if (typeS == typeSerializer.Binary) return new BinarySerializer();
            else if (typeS == typeSerializer.Xml) return new CustomXmlSerializer();
            else throw new Exception("Unknown serializer type");
        }
    }
}
