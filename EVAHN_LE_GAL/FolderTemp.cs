using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVAHN_LE_GAL
{
    [Serializable]
    public class FolderTemp
    {
        public string Name;
        public int Depth; // Depth of the folder, for display purpose
        public DateTime CreationDate;
        public DateTime LastModifiedDate;
        public List<FolderTemp> Folders;
        public List<Contact> Contacts;

        public FolderTemp()
        {
            this.Name = null;
            this.Depth = 0;
            this.CreationDate = DateTime.Now;
            this.LastModifiedDate = DateTime.Now;
            this.Folders = new List<FolderTemp>();
            this.Contacts = new List<Contact>();
        }
        public FolderTemp(Folder original)
        {
            this.Name = original.Name;
            this.Depth = original.Depth;
            this.CreationDate = original.CreationDate;
            this.LastModifiedDate = original.LastModifiedDate;
            this.Folders = new List<FolderTemp>();
            foreach (Folder folder in original.Folders)
            {
                this.Folders.Add(new FolderTemp(folder));
            }
            this.Contacts = original.Contacts;
        }

        public override string ToString()
        {
            return "[D] " + Name + " (creation " + CreationDate.ToString() + " | last modification " + LastModifiedDate.ToString() + " )";
        }
    }
}
