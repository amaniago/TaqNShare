using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace ServeurTaqnshare
{
    class Program
    {
        private static ServiceHost _host;

        static void Main(string[] args)
        {
            _host = new ServiceHost(typeof(ServiceTaqnshare));
            _host.Open();
            Console.WriteLine("Serveur démarré.");

            //Mise en attente du serveur
            while (true)
                if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Enter)
                    break;
            _host.Close();
            Console.WriteLine("Serveur quitté");
            Console.ReadKey();
        }
    }
}
