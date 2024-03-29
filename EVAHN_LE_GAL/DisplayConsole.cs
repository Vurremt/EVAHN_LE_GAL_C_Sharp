﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace EVAHN_LE_GAL
{
    internal class DisplayConsole
    {
        // Affiche le resultat généré par les instructions de l'utilisateur, envoyées par EntriesConsole sous formes d'instructions et d'éléments
        public void displayResult(object sender, string instruction, ElementToSend result)
        {
            switch (instruction)
            {
                case "Heading":
                    Console.Write("$" + result.folder.Name + "> ");
                    break;

                // Display
                case "TooManyArgDisplay":
                    Console.WriteLine("Too many parameters for command 'display'");
                    break;

                case "LineFolder":
                    string tabulation = "";
                    for (int i = 0; i < result.folder.Depth; i++) tabulation += "  ";
                    Console.WriteLine(tabulation + result.folder.ToString());
                    break;

                case "LineContact":
                    string tabulation2 = "";
                    for (int i = 0; i < result.contact.Depth; i++) tabulation2 += "  ";
                    Console.WriteLine(tabulation2 + result.contact.ToString());
                    break;

                // Create
                case "TooManyArgCreate":
                    Console.WriteLine("Too many parameters for command 'create'");
                    break;

                case "TooFewArgCreate":
                    Console.WriteLine("Too few parameters for command 'create'");
                    break;

                case "AskName":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Name of the folder : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "FolderErrorName":
                    Console.WriteLine("The name of the folder must be valid");
                    break;

                case "FolderAlreadyExist":
                    Console.WriteLine("The folder named '" + result.s + "' already exists");
                    break;

                case "AskLastName":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Last Name : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "AskFirstName":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   First Name : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "AskMail":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Mail (___@___.__) : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "AskSociety":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Society : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "AskLink":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Link : \n    [1] Friend\n    [2] Colleague\n    [3] Relation\n    [4] Network\n    [5] NotReferenced\n   -> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "ContactErrorLastName":
                    Console.WriteLine("Last Name must be valid");
                    break;

                case "ContactErrorFirstName":
                    Console.WriteLine("First Name must be valid");
                    break;

                case "ContactAlreadyExist":
                    Console.WriteLine("The contact named '" + result.s + "' already exists");
                    break;

                case "ContactErrorMail":
                    Console.WriteLine("Mail must be valid");
                    break;

                case "ContactErrorSociety":
                    Console.WriteLine("Society must be valid");
                    break;

                case "ContactErrorLink":
                    Console.WriteLine("Link must be valid");
                    break;

                case "CreationImpossible":
                    Console.WriteLine("Creation impossible");
                    break;

                case "CreationInvalid":
                    Console.WriteLine("Error first argument : Specify 'folder' or 'contact' for the creation of a new element");
                    break;

                case "CreationFolder":
                    Console.WriteLine("Creation of folder named '" + result.folder.Name + "'");
                    break;

                case "CreationContact":
                    Console.WriteLine("Creation of contact named '" + result.contact.LastName + " " + result.contact.FirstName + "'");
                    break;

                // Delete
                case "TooManyArgDelete":
                    Console.WriteLine("Too many parameters for command 'delete'");
                    break;

                case "TooFewArgDelete":
                    Console.WriteLine("Too few parameters for command 'delete'");
                    break;

                case "FolderNotExist":
                    Console.WriteLine("The folder named '" + result.s + "' does not exist");
                    break;

                case "DeleteFolder":
                    Console.WriteLine("The folder named '" + result.s + "' has been deleted");
                    break;

                case "ContactNotExist":
                    Console.WriteLine("The contact named '" + result.s + "' does not exist");
                    break;

                case "DeleteContact":
                    Console.WriteLine("The contact named '" + result.s + "' has been deleted");
                    break;

                case "DeleteInvalid":
                    Console.WriteLine("Error first argument : Specify 'folder' or 'contact' for the suppression of an element");
                    break;

                // Cd
                case "WrongArgCd":
                    Console.WriteLine("Command 'cd' required exactly 1 argument");
                    break;

                case "FolderNotFound":
                    Console.WriteLine("Folder " + result.s + " not found");
                    break;

                // Save
                case "SaveInvalid":
                    Console.WriteLine("Error first argument : Specify 'xml' or 'bin' for the type of storage");
                    break;

                case "AskFileSave":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Name of the file to save data (without .bin or .xml) : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "AskKey":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Key for crypto : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "SaveSucceed":
                    Console.WriteLine("The state of the application has been successfully saved");
                    break;

                case "SaveImpossible":
                    Console.WriteLine("Impossible to open the file of data");
                    break;

                // Load
                case "LoadInvalid":
                    Console.WriteLine("Error first argument : Specify 'xml' or 'bin' for the type of storage");
                    break;

                case "AskFileLoad":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("   Name of the file to save data (without .bin or .xml) : ");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case "LoadSucceed":
                    Console.WriteLine("The state of the application has been successfully loaded");
                    break;

                case "LoadImpossible":
                    Console.WriteLine("Impossible to open the file : Wrong key entered");
                    break;

                // Help
                case "TooManyArgHelp":
                    Console.WriteLine("Too many parameters for command 'help'");
                    break;

                case "Manual":
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n -- Actions of the application --\n");

                    // display
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("display ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[all] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": Display tree structure from the current folder ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("/ from the root folder");

                    // create
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("create");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(" [folder] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to create a new folder in the current folder");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("       [contact] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to create a new contact in the current folder (Structure of email : ___ @ ___ . __)");

                    // delete
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("delete");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(" [folder] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to delete a folder with its name (must be executed in its parent folder)");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("       [contact] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to delete a contact (Last Name and First Name)");

                    // cd
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("cd ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[./ or ../ or Root/] [path, folders separated by '/'] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Navigate in the tree with a path in argument");

                    // save
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("save ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(" [xml] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to save data in a xml file (without .xml)");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("      [binary] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to save data in a binary file (without .bin)");

                    // load
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("load ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(" [xml] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to load data from a xml file (without .xml)");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("      [binary] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Open the panel to load data from a binary file (without .bin)");

                    // quit
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("clear ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Clear the terminal");

                    // quit
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("quit ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": Quit the application without saving");


                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Press any key to continue... ");
                    Console.ReadKey();
                    Console.Clear();
                    break;

                // Quit
                case "TooManyArgQuit":
                    Console.WriteLine("Too many parameters for command 'quit'");
                    break;

                case "Quiting":
                    changeColor(this, ConsoleColor.Green);
                    Console.WriteLine("\nQuiting the application...");
                    break;

                // Other
                case "UnknownParameter":
                    Console.WriteLine("Unknown parameter");
                    break;

                case "UnknownCommand":
                    Console.WriteLine("Unknown command");
                    break;

                case "LineBreak":
                    Console.WriteLine();
                    break;

                case "Clear":
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Instruction not specified");
                    break;
            }
        }

        // Permet à EntriesConsole à demander de changer de couleur au terminal
        public void changeColor(object sender, ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
    }
}
