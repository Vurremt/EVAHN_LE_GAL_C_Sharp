using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVAHN_LE_GAL
{
    public class Folder
    {
        public string Name;
        public int Depth; // Profondeur dans l'arborescence du contact, pour l'affichage uniquement
        public DateTime CreationDate;
        public DateTime LastModifiedDate;
        public Folder Parent;
        public List<Folder> Folders;
        public List<Contact> Contacts;


        public Folder(string name, Folder parent)
        {
            this.Name = name;
            if(parent != null) this.Depth = parent.Depth + 1;
            else this.Depth = 0;
            this.CreationDate = DateTime.Now;
            this.LastModifiedDate = DateTime.Now;
            this.Parent = parent;
            this.Folders = new List<Folder>();
            this.Contacts = new List<Contact>();
        }

        // Permet de transformer un FolderTemp en Folder (par récursivité) en lui associant un parent
        public Folder(FolderTemp copy, Folder parent)
        {
            this.Name = copy.Name;
            this.Depth = copy.Depth;
            this.CreationDate = copy.CreationDate;
            this.LastModifiedDate = copy.LastModifiedDate;
            this.Parent = parent;
            this.Folders = new List<Folder>();
            foreach (FolderTemp folder in copy.Folders)
            {
                this.Folders.Add(new Folder(folder, this));
            }
            this.Contacts = copy.Contacts;
        }

        // Pour l'affichage console des principales informations d'un FolderTemp
        public override string ToString()
        {
            return "[D] " + Name + " (creation " + CreationDate.ToString() + " | last modification " + LastModifiedDate.ToString() + " )";
        }
    }
}
