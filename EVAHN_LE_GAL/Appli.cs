using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EVAHN_LE_GAL
{
    // Une structure qui est un dossier, un contact ou une string (les union sont très problematiques en C#, je ne sais pas les faire)
    // On perd un petit peu de mémoire, mais deja cette place est négligeable, et les champs non utilisés sont mis à null
    public class ElementToSend
    {
        public Folder folder;
        public Contact contact;
        public string s;

        public ElementToSend(Folder folder)
        {
            this.folder = folder;
            this.contact = null;
            this.s = null;
        }

        public ElementToSend(Contact contact)
        {
            this.folder = null;
            this.contact = contact;
            this.s = null;
        }

        public ElementToSend(String s)
        {
            this.folder = null;
            this.contact = null;
            this.s = s;
        }
    }

    // La base de l'application, un objet avec des fonctions de modifications internes, sans interaction directe avec les utilisateurs (qui doivent passer par l'interface puis l'EntriesConsole, qui est l'élément qui gère l'application)
    public class Appli
    {
        public Folder root;
        public Folder current;
        public string pathMyDocuments;

        public Appli() {
            root = new Folder("Root", null);
            root.Parent = root; // Le parent de Root est lui même, pour pouvoir faire cd ../../../../ et retomber sur Root
            current = root;
            pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Appli_c_sharp_Evahn_LE_GAL"; // L'application connait à tout moment le chemin d'accès au repertoir de sauvegarde
            if(!Directory.Exists(pathMyDocuments)) Directory.CreateDirectory(pathMyDocuments);
        }

        // Compare deux fichiers par noms
        private static int CompareFolderByName(Folder f1, Folder f2)
        {
            return f1.Name.CompareTo(f2.Name);
        }

        // Creation d'un nouveau dossier
        public Folder createFolder(string name) {
            try
            {
                // Nouveau dossier
                Folder newFolder = new Folder(name, current);
                current.Folders.Add(newFolder);
                current.Folders.Sort(CompareFolderByName);

                // Met à jour les dossiers parents avec la date actuelle
                current.LastModifiedDate = DateTime.Now;
                do
                {
                    current = current.Parent;
                    current.LastModifiedDate = DateTime.Now;
                } while (current != root);

                // Retourne le nouveau dossier
                current = newFolder;
                return newFolder;
            } catch(Exception e)
            {
                return null;
            }

        }

        // Compare deux contact par noms (Nom de famille + Prenom)
        private static int CompareContactByName(Contact c1, Contact c2)
        {
            string s1 = c1.LastName + " " + c1.FirstName;
            string s2 = c2.LastName + " " + c2.FirstName;
            return s1.CompareTo(s2);
        }

        // Creation d'un nouveau contact
        public Contact createContact(string LastName, string FirstName, string Mail, string Society, Link Link)
        {
            try
            {
                // Nouveau contact
                Contact newContact = new Contact(current.Depth, LastName, FirstName, Mail, Society, Link);
                current.Contacts.Add(newContact);
                current.Contacts.Sort(CompareContactByName);

                // Met à jour les dossiers parents avec la date actuelle
                Folder temp = current;
                current.LastModifiedDate = DateTime.Now;
                do
                {
                    current = current.Parent;
                    current.LastModifiedDate = DateTime.Now;
                } while (current != root);

                // Retourne le nouveau contact
                current = temp;
                return newContact;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        // Yield les éléments de l'arborescence ligne par ligne avec l'algorithme de parcours en profondeur (avec pile, non recursif)
        public IEnumerable<ElementToSend> displayTree(Folder folderRoot)
        {
            Stack<Folder> stackFolders = new Stack<Folder>();
            stackFolders.Push(folderRoot);

            while(stackFolders.Count > 0){

                Folder folder = stackFolders.Pop();
                yield return new ElementToSend(folder);

                foreach (Contact c in folder.Contacts)
                {
                    yield return new ElementToSend(c);
                }

                foreach (Folder f in folder.Folders){
                    stackFolders.Push(f);
                }
            }
        }

        private Folder searchName(string name)
        {
            foreach(Folder folder in current.Folders)
            {
                if(folder.Name.Equals(name)) return folder;
            }
            return null;
        }

        public bool FolderAlreadyExist(string name)
        {
            foreach (Folder folder in current.Folders)
            {
                if (folder.Name.Equals(name)) return true;
            }
            return false;
        }

        public bool ContactAlreadyExist(string name)
        {
            foreach (Contact contact in current.Contacts)
            {
                string n = contact.LastName + " " + contact.FirstName;
                if (n.Equals(name)) return true;
            }
            return false;
        }

        // prend un chemin d'accès en paramètre (sous forme de tableau de noms de dossiers) et parcours l'arborescence, élément après élément, jusqu'à la fin ou jusqu'à une erreur
        public string navigate(string[] path) {
            Folder save = current;
            int i = 0;
            if (path[0] == "Root")
            {
                current = root;
                i = 1;
            }
            if (path[path.Length - 1] == "") path = path.Take(path.Length - 1).ToArray();
            for (; i < path.Length; i++)
            {
                if (path[i] == ".") current = current; // On reste dans le même dossier
                else if (path[i] == "..") current = current.Parent; // On va dans le dossier parent
                else if (searchName(path[i]) != null)
                {
                    current = searchName(path[i]); // On va dans le sous dossier s'il existe
                }
                else
                {
                    current = save; // Sinon on revient à zéro et on retourne une erreur (le nom du dossier introuvable)
                    return path[i];
                }
            }
            return null;
        }

        // Supprime un dossier avec son nom dans le repertoire courant
        public void deleteFolder(string name)
        {
            Folder folderToDelete = searchName(name);

            // On supprime par recursion ses sous dossiers
            foreach (Folder folder in folderToDelete.Folders.ToList())
            {
                current = folderToDelete;
                deleteFolder(folder.Name);
                current = current.Parent;
            }

            folderToDelete.Contacts.Clear();

            current.Folders.Remove(folderToDelete);

            // Update parent folders with lastest modifications
            Folder temp = current;
            current.LastModifiedDate = DateTime.Now;
            do
            {
                current = current.Parent;
                current.LastModifiedDate = DateTime.Now;
            } while (current != root);
            current = temp;
        }

        // Supprimer un contact avec son nom
        public void deleteContact(string name)
        {
            Contact contact = null;
            foreach (Contact c in current.Contacts)
            {
                if (c.LastName + " " + c.FirstName == name) contact = c;
            }
            if(contact != null)
                current.Contacts.Remove(contact);

            // Update parent folders with lastest modifications
            Folder temp = current;
            current.LastModifiedDate = DateTime.Now;
            do
            {
                current = current.Parent;
                current.LastModifiedDate = DateTime.Now;
            } while (current != root);
            current = temp;
        }

        // Sauvegarder l'état actuel de l'arborescence avec un nom de fichier, un clé de cryptage et un type de sérialisation
        public void save(string filename, typeSerializer typeS, string key)
        {
            filename = pathMyDocuments + "/" + filename;

            // Copy current tree in FolderTemp (without field Parent for circular depencies)
            FolderTemp copy = new FolderTemp(root);

            // Serialize
            ISerializer serializer = FactorySerializer.GetSerializer(typeS);

            serializer.Serialize(filename, copy, key);
        }

        // Carger l'état d'une l'arborescence avec un nom de fichier, un clé de cryptage et un type de sérialisation
        public bool load(string filename, typeSerializer typeS, string key)
        {
            filename = pathMyDocuments + "/" + filename;
            if (!File.Exists(filename)) return false;

            // Deserialize
            ISerializer serializer = FactorySerializer.GetSerializer(typeS);

            FolderTemp data = serializer.Deserialize(filename, key);
            if(data == null) return false;

            // Remplace state of the application by data loaded
            root = new Folder(data, null);
            root.Parent = root;
            current = root;
            return true;
        }
    }
}
