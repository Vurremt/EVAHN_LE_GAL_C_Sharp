using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVAHN_LE_GAL
{
    // Dossier sans parent pour eviter les dependances cycliques
    [Serializable]
    public class FolderTemp
    {
        public string Name;
        public int Depth; // Profondeur dans l'arborescence du contact, pour l'affichage uniquement
        public DateTime CreationDate;
        public DateTime LastModifiedDate;
        public List<FolderTemp> Folders;
        public List<Contact> Contacts;

        //Il faut un constructeur sans argument pour la serialisation
        public FolderTemp()
        {
            this.Name = null;
            this.Depth = 0;
            this.CreationDate = DateTime.Now;
            this.LastModifiedDate = DateTime.Now;
            this.Folders = new List<FolderTemp>();
            this.Contacts = new List<Contact>();
        }

        // Permet de transformer un Folder en FolderTemp (par récursivité)
        public FolderTemp(Folder original)
        {
            this.Name = original.Name;
            this.Depth = original.Depth;
            this.CreationDate = original.CreationDate;
            this.LastModifiedDate = original.LastModifiedDate;
            this.Folders = new List<FolderTemp>();
            // par recursivité, on transforme tous les sous dossiers Folder en FolderTemp
            foreach (Folder folder in original.Folders)
            {
                this.Folders.Add(new FolderTemp(folder));
            }
            this.Contacts = original.Contacts;
        }

        // Pour l'affichage console des principales informations d'un FolderTemp
        public override string ToString()
        {
            return "[D] " + Name + " (creation " + CreationDate.ToString() + " | last modification " + LastModifiedDate.ToString() + " )";
        }
    }
}
