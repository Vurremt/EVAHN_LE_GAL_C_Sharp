using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Mail;
using System.Xml.Serialization;
using System.Xml;

namespace EVAHN_LE_GAL
{
    internal class EntriesConsole
    {
        public Appli application;

        // Event handler which send an element with an instruction of how to display it
        public delegate void newInstructionEventHandler(object sender, string instruction, ElementToSend element);
        public event newInstructionEventHandler newInstruction;

        // Event handler which send the color to display in the console
        public delegate void changeColorEventHandler(object sender, ConsoleColor color);
        public event changeColorEventHandler changeColor;


        public EntriesConsole()
        {
            application = new Appli();
        }

        // A changer pour récuperer instructions d'une partie graphique plutot que des lignes en consoles
        public string recupInstruction()
        {
            return Console.ReadLine();
        }

        // Stack Overflow
        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }


        // Read instructions in the console, call appropriate functions and send the results line by line in an instruction
        // Return if the application must close itself
        public bool readCommandUser(){
            newInstruction(this, "Heading", new ElementToSend(application.current));

            // Read the instruction and split possibles arguments
            string line = recupInstruction();
            changeColor(this, ConsoleColor.White);
            string[] command = line.Split(' ');

            // Follow the first instruction, check after for each parameters if it's ok
            switch (command[0]){

                case "display":
                    if (command.Length > 2) newInstruction(this, "TooManyArgDisplay", null);
                    else if (command.Length == 2)
                    {
                        if (command[1] == "all")
                        {
                            foreach (ElementToSend element in application.displayTree(application.root))
                            {
                                if(element.folder == null) newInstruction(this, "LineContact", element);
                                else newInstruction(this, "LineFolder", element);
                            }
                        }
                        else newInstruction(this, "UnknownParameter", null);
                    }
                    else {
                        foreach (ElementToSend element in application.displayTree(application.current))
                        {
                            if (element.folder == null)
                            {
                                int temp = element.contact.Depth;
                                element.contact.Depth -= application.current.Depth;
                                newInstruction(this, "LineContact", element);
                                element.contact.Depth = temp;
                            }
                            else
                            {
                                int temp = element.folder.Depth;
                                element.folder.Depth -= application.current.Depth;
                                newInstruction(this, "LineFolder", element);
                                element.folder.Depth = temp;
                            }
                        }
                    }
                    break;

                case "create":
                    if (command.Length < 2) newInstruction(this, "TooFewArgCreate", null);
                    else if(command.Length > 2) newInstruction(this, "TooManyArgCreate", null);
                    else
                    {
                        if (command[1] == "folder")
                        {
                            newInstruction(this, "AskName", null);
                            string name = recupInstruction();

                            if (name == null || name == "")
                            {
                                newInstruction(this, "FolderErrorName", null);
                                break;
                            }
                            if (application.FolderAlreadyExist(name))
                            {
                                newInstruction(this, "FolderAlreadyExist", new ElementToSend(name));
                                break;
                            }
                            Folder newFolder = application.createFolder(name);

                            if (newFolder != null) newInstruction(this, "CreationFolder", new ElementToSend(newFolder));
                            else newInstruction(this, "CreationImpossible", null);
                        }
                        else if (command[1] == "contact")
                        {
                            newInstruction(this, "AskLastName", null);
                            string LastName = recupInstruction();
                            if (LastName == null || LastName == "")
                            {
                                newInstruction(this, "ContactErrorLastName", null);
                                break;
                            }

                            newInstruction(this, "AskFirstName", null);
                            string FirstName = recupInstruction();
                            if (FirstName == null || FirstName == "")
                            {
                                newInstruction(this, "ContactErrorFirstName", null);
                                break;
                            }

                            if (application.ContactAlreadyExist(LastName + " " + FirstName))
                            {
                                newInstruction(this, "ContactAlreadyExist", new ElementToSend(LastName + " " + FirstName));
                                break;
                            }

                            newInstruction(this, "AskMail", null);
                            string Mail = recupInstruction();
                            if (Mail == null || Mail == "" || !IsValidEmail(Mail))
                            {
                                newInstruction(this, "ContactErrorMail", null);
                                break;
                            }

                            newInstruction(this, "AskSociety", null);
                            string Society = recupInstruction();
                            if (Society == null || Society == "")
                            {
                                newInstruction(this, "ContactErrorSociety", null);
                                break;
                            }

                            newInstruction(this, "AskLink", null);
                            int i = 5;
                            try
                            {
                                i = Int32.Parse(recupInstruction());
                                if (i < 1 || i > 5)
                                {
                                    newInstruction(this, "ContactErrorLink", null);
                                    break;
                                }
                            }
                            catch (FormatException)
                            {
                                newInstruction(this, "ContactErrorLink", null);
                                break;
                            }
                            Link link = (Link) (i-1);

                            
                            Contact newContact = application.createContact(LastName, FirstName, Mail, Society, link);
                            if (newContact != null) newInstruction(this, "CreationContact", new ElementToSend(newContact));
                            else newInstruction(this, "CreationImpossible", null);
                        } 
                        else
                        {
                            newInstruction(this, "CreationInvalid", null);
                        }
                    }
                    break;

                case "delete":
                    if (command.Length < 2) newInstruction(this, "TooFewArgDelete", null);
                    else if (command.Length > 2) newInstruction(this, "TooManyArgDelete", null);
                    else
                    {
                        if (command[1] == "folder")
                        {
                            newInstruction(this, "AskName", null);
                            string name = recupInstruction();

                            if (name == null || name == "")
                            {
                                newInstruction(this, "FolderErrorName", null);
                                break;
                            }
                            if (!application.FolderAlreadyExist(name))
                            {
                                newInstruction(this, "FolderNotExist", new ElementToSend(name));
                                break;
                            }

                            application.deleteFolder(name);
                            newInstruction(this, "DeleteFolder", new ElementToSend(name));
                        }
                        else if (command[1] == "contact")
                        {
                            newInstruction(this, "AskLastName", null);
                            string LastName = recupInstruction();
                            if (LastName == null || LastName == "")
                            {
                                newInstruction(this, "ContactErrorLastName", null);
                                break;
                            }

                            newInstruction(this, "AskFirstName", null);
                            string FirstName = recupInstruction();
                            if (FirstName == null || FirstName == "")
                            {
                                newInstruction(this, "ContactErrorFirstName", null);
                                break;
                            }

                            if (!application.ContactAlreadyExist(LastName + " " + FirstName))
                            {
                                newInstruction(this, "ContactNotExist", new ElementToSend(LastName + " " + FirstName));
                                break;
                            }

                            application.deleteContact(LastName + " " + FirstName);
                            newInstruction(this, "DeleteContact", new ElementToSend(LastName + " " + FirstName));
                        }
                        else
                        {
                            newInstruction(this, "DeleteInvalid", null);
                        }
                    }
                    break;

                case "cd":
                    if (command.Length != 2) newInstruction(this, "WrongArgCd", null);
                    else
                    {
                        string[] path = command[1].Split('/');
                        if (application.navigate(path) != null)
                        {
                            newInstruction(this, "FolderNotFound", new ElementToSend(application.navigate(path)));
                        }
                    }
                    break;

                case "save":
                    if (command.Length != 2) newInstruction(this, "SaveInvalid", null);
                    else
                    {
                        if (command[1] == "binary")
                        {
                            newInstruction(this, "AskFileSave", null);
                            string name = recupInstruction() + ".bin";

                            newInstruction(this, "AskKey", null);
                            string key = recupInstruction();

                            try
                            {
                                application.save(name, typeSerializer.Binary, key);
                                newInstruction(this, "SaveSucceed", null);
                            }
                            catch (System.Security.Cryptography.CryptographicException e)
                            {
                                newInstruction(this, "SaveImpossible", null);
                            }
                        }
                        else if (command[1] == "xml")
                        {
                            newInstruction(this, "AskFileLoad", null);
                            string name = recupInstruction() + ".xml";

                            newInstruction(this, "AskKey", null);
                            string key = recupInstruction();

                            try
                            {
                                application.save(name, typeSerializer.Xml, key);
                                newInstruction(this, "SaveSucceed", null);
                            }
                            catch(System.Security.Cryptography.CryptographicException e)
                            {
                                newInstruction(this, "SaveImpossible", null);
                            }
                        }
                        else
                        {
                            newInstruction(this, "SaveInvalid", null);
                        }
                    }
                    break;

                case "load":
                    if (command.Length != 2) newInstruction(this, "LoadInvalid", null);
                    else
                    {
                        if (command[1] == "binary")
                        {
                            newInstruction(this, "AskFileLoad", null);
                            string name = recupInstruction() + ".bin";

                            newInstruction(this, "AskKey", null);
                            string key = recupInstruction();

                            try
                            {
                                application.load(name, typeSerializer.Binary, key);
                                newInstruction(this, "LoadSucceed", null);
                            }
                            catch (System.Security.Cryptography.CryptographicException e)
                            {
                                newInstruction(this, "LoadImpossible", null);
                            }
                        }
                        else if (command[1] == "xml")
                        {
                            newInstruction(this, "AskFileLoad", null);
                            string name = recupInstruction() + ".xml";

                            newInstruction(this, "AskKey", null);
                            string key = recupInstruction();

                            try
                            {
                                application.load(name, typeSerializer.Xml, key);
                                newInstruction(this, "LoadSucceed", null);
                            }
                            catch (System.Security.Cryptography.CryptographicException e)
                            {
                                newInstruction(this, "LoadImpossible", null);
                            }
                        }
                        else
                        {
                            newInstruction(this, "LoadInvalid", null);
                        }
                    }
                    break;

                case "help":
                    if (command.Length > 1) newInstruction(this, "TooManyArgHelp", null);
                    else newInstruction(this, "Manual", null);
                    break;

                case "clear":
                    newInstruction(this, "Clear", null);
                    break;

                case "quit":
                    if (command.Length > 1) newInstruction(this, "TooManyArgQuit", null);
                    else {
                        newInstruction(this, "Quiting", null);
                        return true;
                    }
                    break;

                default:
                    newInstruction(this, "UnknownCommand", null);
                    break;
            }

            changeColor(this, ConsoleColor.Green);
            newInstruction(this, "LineBreak", null);

            return false;

        }

    }

}
