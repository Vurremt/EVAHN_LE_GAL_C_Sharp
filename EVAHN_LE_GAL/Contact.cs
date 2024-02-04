using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EVAHN_LE_GAL
{
    public enum Link
    {
        Friend,
        Colleague,
        Relation,
        Network,
        NotReferenced
    }

    static class LinkMethods
    {
        public static String toString(this Link l)
        {
            switch (l)
            {
                case Link.Friend:
                    return "Friend";
                case Link.Colleague:
                    return "Colleague";
                case Link.Relation:
                    return "Relation";
                case Link.Network:
                    return "Network";
                default:
                    return "NotReferenced";
            }
        }
    }

    [Serializable]
    public class Contact
    {
        public int Depth; // Depth of the contact, for display purpose
        public string LastName;
        public string FirstName;
        public string Mail;
        public string Society;
        public Link Link;
        public DateTime CreationDate;

        public Contact()
        {
            this.Depth = 0;
            this.LastName = null;
            this.FirstName = null;
            this.Mail = null;
            this.Society = null;
            this.Link = Link.NotReferenced;
            this.CreationDate = DateTime.Now;
        }

        public Contact(int Depth, string LastName, string FirstName, string Mail, string Society, Link Link)
        {
            this.Depth = Depth;
            this.LastName = LastName;
            this.FirstName = FirstName;
            this.Mail = Mail;
            this.Society = Society;
            this.Link = Link;
            this.CreationDate = DateTime.Now;
        }

        public override string ToString()
        {
            return "|  [C] " + LastName + " " + FirstName + " (" + Society + "), " + Mail + ", Link : " + Link.ToString() + " (creation " + CreationDate.ToString() + " )";
        }
    }
}
