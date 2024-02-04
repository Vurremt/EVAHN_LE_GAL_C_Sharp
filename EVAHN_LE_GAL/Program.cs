using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVAHN_LE_GAL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Creation des différents éléments
            EntriesConsole entryConsole = new EntriesConsole();
            DisplayConsole displayConsole = new DisplayConsole();
            
            // Abonnement aux listeners
            entryConsole.newInstruction += displayConsole.displayResult;
            entryConsole.changeColor += displayConsole.changeColor;


            bool needToQuit = false;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(" --- Application C# Evahn LE GAL ---");
            Console.WriteLine("-type command 'help' to display possible actions\n");

            // Lance et fait tourner l'application entière
            while (!needToQuit) {
                needToQuit = entryConsole.readCommandUser();
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
