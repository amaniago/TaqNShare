using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Début");

            using (var db = new db_taqnshareEntities())
            {
                // Create and save a new Blog 
                /*Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();

                var blog = new Blog { Name = name };
                db.Blogs.Add(blog);
                db.SaveChanges();*/

                // Display all Blogs from the database 
                var query = from b in db.utilisateurs
                            orderby b.id_utilisateur
                            select b;

                Console.WriteLine("Liste des idUsers");
                foreach (var item in query)
                {
                    Console.WriteLine(item.id_utilisateur);
                }

                Console.WriteLine("Fin");
                Console.ReadKey();
            } 
        }
    }
}
