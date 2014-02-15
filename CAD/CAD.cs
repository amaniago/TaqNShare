using System;
using System.Linq;

namespace CAD
{
    public class Cad
    {
        public string GetId()
        {
            try
            {
                var db = new db_taqnshareEntities();

                var query = from b in db.utilisateurs
                            orderby b.id_utilisateur
                            select b;

                var listeIdUtilisateur = query.Select(item => item.id_utilisateur).ToList();
                return listeIdUtilisateur.ElementAt(2);
            }
            catch (Exception e)
            {
                return e.ToString();
            }  
        }
    }
}
