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
            /*float score = 0;
            IServiceTaqnshare test = new ServiceTaqnshare();
            ObservableCollection<Utilisateur> u = test.RecupererClassement();

            foreach (Utilisateur u2 in u)
            {
                Console.Write(u2.nom_utilisateur);
                Console.Write(" ");
                Console.Write(u2.prenom_utilisateur);
                Console.Write(" ");
                score = (float) (u2.score_total_utilisateur/u2.nombre_partie_utilisateur);
                Console.WriteLine(score);
            }

            Console.ReadKey();*/


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
