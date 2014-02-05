using System;

namespace CAD
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Début");

            //db_taqnshareEntities db = new db_taqnshareEntities();
            // Create and save a new Blog 
            /*Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();

            var blog = new Blog { Name = name };
            db.Blogs.Add(blog);
            db.SaveChanges();*/

            // Display all Blogs from the database 
            /*var query = from b in db.utilisateurs
                        orderby b.id_utilisateur
                        select b;

            Console.WriteLine("Liste des idUsers");
            foreach (var item in query)
            {
                Console.WriteLine(item.id_utilisateur);
            }*/

            var cad = new Cad();

            Console.WriteLine(cad.GetId());

            Console.WriteLine("Fin");
            Console.ReadKey();
            
        }
    }
}
