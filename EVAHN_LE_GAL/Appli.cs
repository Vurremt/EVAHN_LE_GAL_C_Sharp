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
    // A structure which is a folder, a contact or a string (union is very problematic in C#, i couldn't do it here so we lose some memory, but negligible)
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

    // The base of the application, an object with some functions of modification internally, without interaction with users
    public class Appli
    {
        public Folder root;
        public Folder current;
        public string pathMyDocuments;

        public Appli() {
            root = new Folder("Root", null);
            root.Parent = root;
            current = root;
            pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Appli_c_sharp_Evahn_LE_GAL";
            if(!Directory.Exists(pathMyDocuments)) Directory.CreateDirectory(pathMyDocuments);
        }

        private static int CompareFolderByName(Folder f1, Folder f2)
        {
            return f1.Name.CompareTo(f2.Name);
        }

        // Creation of a new folder
        public Folder createFolder(string name) {
            try
            {
                // New folder
                Folder newFolder = new Folder(name, current);
                current.Folders.Add(newFolder);
                current.Folders.Sort(CompareFolderByName);

                // Update parent folders with lastest modifications
                current.LastModifiedDate = DateTime.Now;
                do
                {
                    current = current.Parent;
                    current.LastModifiedDate = DateTime.Now;
                } while (current != root);

                // Return the new folder
                current = newFolder;
                return newFolder;
            } catch(Exception e)
            {
                return null;
            }

        }

        private static int CompareContactByName(Contact c1, Contact c2)
        {
            string s1 = c1.LastName + " " + c1.FirstName;
            string s2 = c2.LastName + " " + c2.FirstName;
            return s1.CompareTo(s2);
        }

        // Creation of a new contact
        public Contact createContact(string LastName, string FirstName, string Mail, string Society, Link Link)
        {
            try
            {
                // New contact
                Contact newContact = new Contact(current.Depth, LastName, FirstName, Mail, Society, Link);
                current.Contacts.Add(newContact);
                current.Contacts.Sort(CompareContactByName);

                // Update parent folders with lastest modifications
                Folder temp = current;
                current.LastModifiedDate = DateTime.Now;
                do
                {
                    current = current.Parent;
                    current.LastModifiedDate = DateTime.Now;
                } while (current != root);

                // Return the new folder
                current = temp;
                return newContact;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        // Yield elemens of the tree line by line with Depth-first search
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
                if (path[i] == ".") current = current;
                else if (path[i] == "..") current = current.Parent;
                else if (searchName(path[i]) != null)
                {
                    current = searchName(path[i]);
                }
                else
                {
                    current = save;
                    return path[i];
                }
            }
            return null;
        }

        public void deleteFolder(string name)
        {
            Folder folderToDelete = searchName(name);

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

        public void save(string filename, typeSerializer typeS, string key)
        {
            filename = pathMyDocuments + "/" + filename;

            // Copy current tree in FolderTemp (without field Parent for circular depencies)
            FolderTemp copy = new FolderTemp(root);

            // Serialize
            ISerializer serializer = FactorySerializer.GetSerializer(typeS);

            serializer.Serialize(filename, copy, key);
        }

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
