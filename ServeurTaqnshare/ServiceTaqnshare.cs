using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using ServeurTaqnshare.ClasseDeService;

namespace ServeurTaqnshare
{
    public class ServiceTaqnshare : IServiceTaqnshare
    {
        /// <summary>
        /// Méthode permettant d'enregistrer le score de l'utilisateur
        /// </summary>
        /// <param name="utilisateurCourant"></param>
        /// <param name="scorePartie"></param>
        /// <returns></returns>
        public bool EnregistrerScore(Utilisateur utilisateurCourant, int scorePartie)
        {
            try
            {
                var db = new TaqnshareEntities();

                var utilisateur = (from u in db.Utilisateurs
                                   where u.id_utilisateur == utilisateurCourant.id_utilisateur
                                   select u).SingleOrDefault();

                if (utilisateur != null)
                {
                    utilisateur.nombre_partie_utilisateur += 1;
                    utilisateur.score_total_utilisateur += scorePartie;
                }
                else
                {
                    Utilisateur nouvelUtilisateur = new Utilisateur
                    {
                        id_utilisateur = utilisateurCourant.id_utilisateur,
                        prenom_utilisateur = utilisateurCourant.prenom_utilisateur,
                        nom_utilisateur = utilisateurCourant.nom_utilisateur,
                        nombre_partie_utilisateur = 1,
                        score_total_utilisateur = scorePartie
                    };
                    db.Utilisateurs.Add(nouvelUtilisateur);
                }
                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Méthode permettant de créer un défi et de stocker l'image associer
        /// </summary>
        /// <param name="partieUtilisateur"></param>
        /// <param name="compositionTaquin"></param>
        /// <param name="imageDefi"></param>
        /// <returns></returns>
        public bool CreerDefi(Defi partieUtilisateur, List<Composer> compositionTaquin, byte[] imageDefi)
        {
            try
            {

                var db = new TaqnshareEntities();

                var nouveauDefi = db.Defis.Add(partieUtilisateur);
                db.SaveChanges();

                nouveauDefi.chemin_image_defi = (Directory.GetCurrentDirectory() + "\\ImagesWebService\\Image" + nouveauDefi.id_defi + ".jpg");
                db.Defis.AddOrUpdate(nouveauDefi);
                db.SaveChanges();

                foreach (Composer composer in compositionTaquin)
                {
                    composer.id_defi = nouveauDefi.id_defi;
                }

                db.Composers.AddRange(compositionTaquin);
                db.SaveChanges();

                MemoryStream ms = new MemoryStream(imageDefi);
                FileStream fs = new FileStream(nouveauDefi.chemin_image_defi, FileMode.Create);

                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Méthode permettant de récupérer un défi et son image
        /// </summary>
        /// <param name="idDefi"></param>
        /// <returns></returns>
        public DefiService RecupererDefi(int idDefi)
        {
            var db = new TaqnshareEntities();

            var defi = (from defis in db.Defis
                        where defis.id_defi == idDefi
                        select defis).Single();

            var nombreFiltre = (from c in db.Composers
                                where c.id_defi == idDefi && c.id_filtre != 0
                                group c by c.id_filtre into cgroup
                                select new
                                {
                                    cgroup.Key
                                }).Count();

            byte[] imageDefi = File.ReadAllBytes(defi.chemin_image_defi);

            var utilisateur = (from u in db.Utilisateurs
                               where u.id_utilisateur == defi.id_utilisateur
                               select u).SingleOrDefault();

            DefiService retour = new DefiService(defi, imageDefi, nombreFiltre, utilisateur);

            return retour;
        }

        /// <summary>
        /// Méthode permettant de modifier un défi lorsque celui-ci a été accepter par l'adversaire
        /// </summary>
        /// <param name="defiTermine"></param>
        /// <param name="utilisateurCourant"></param>
        /// <returns></returns>
        public bool ModifierDefi(Defi defiTermine, Utilisateur utilisateurCourant)
        {
            try
            {
                var db = new TaqnshareEntities();

                var defi = (from defis in db.Defis
                            where defis.id_defi == defiTermine.id_defi
                            select defis).Single();

                defi.resolu = defiTermine.resolu;
                defi.score_adversaire_defi = defiTermine.score_adversaire_defi;

                db.Defis.AddOrUpdate(defi);
                db.SaveChanges();

                var utilisateur = (from u in db.Utilisateurs
                                   where u.id_utilisateur == utilisateurCourant.id_utilisateur
                                   select u).SingleOrDefault();

                if (utilisateur != null)
                {
                    utilisateur.nombre_partie_utilisateur += 1;
                    utilisateur.score_total_utilisateur += defiTermine.score_adversaire_defi;
                }
                else
                {
                    Utilisateur nouvelUtilisateur = new Utilisateur
                    {
                        id_utilisateur = utilisateurCourant.id_utilisateur,
                        prenom_utilisateur = utilisateurCourant.prenom_utilisateur,
                        nom_utilisateur = utilisateurCourant.nom_utilisateur,
                        nombre_partie_utilisateur = 1,
                        score_total_utilisateur = defiTermine.score_adversaire_defi
                    };
                    db.Utilisateurs.Add(nouvelUtilisateur);
                }
                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Méthode permettant de décliner un défi
        /// </summary>
        /// <param name="idDefiDecline"></param>
        /// <returns></returns>
        public bool DeclinerDefi(int idDefiDecline)
        {
            try
            {
                var db = new TaqnshareEntities();

                var defi = (from defis in db.Defis
                            where defis.id_defi == idDefiDecline
                            select defis).Single();

                defi.resolu = true;
                defi.score_adversaire_defi = 0;

                db.Defis.AddOrUpdate(defi);
                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Méthode permettant de récupérer tous les défis en attente de l'utilisateur
        /// </summary>
        /// <param name="idUtilisateur"></param>
        /// <returns></returns>
        public List<DefiService> RecupererDefisEnAttente(string idUtilisateur)
        {

            var db = new TaqnshareEntities();

            List<Defi> defis = (from d in db.Defis
                                where d.id_adversaire_defi == idUtilisateur && d.resolu == false
                                select d).ToList();

            List<DefiService> defisRetour = new List<DefiService>();
            foreach (var defi in defis)
            {
                var utilisateur = (from u in db.Utilisateurs
                                   where u.id_utilisateur == defi.id_utilisateur
                                   select u).SingleOrDefault();

                defisRetour.Add(new DefiService(defi, utilisateur));
            }

            return defisRetour;
        }

        /// <summary>
        /// Méthode permettant de récupérer tous les défis terminés concernant l'utilisateur
        /// </summary>
        /// <param name="idUtilisateur"></param>
        /// <returns></returns>
        public List<DefiService> RecupererDefisUtilisateur(string idUtilisateur)
        {

            var db = new TaqnshareEntities();

            var defis = from d in db.Defis
                        where d.id_utilisateur == idUtilisateur || d.id_adversaire_defi == idUtilisateur
                        select d;

            List<DefiService> defisRetour = new List<DefiService>();
            foreach (var defi in defis)
            {
                if ((bool)defi.resolu)
                {
                    var utilisateur = (from u in db.Utilisateurs
                                       where u.id_utilisateur == defi.id_utilisateur
                                       select u).SingleOrDefault();

                    var adversaire = (from a in db.Utilisateurs
                                      where a.id_utilisateur == defi.id_adversaire_defi
                                      select a).SingleOrDefault();

                    defisRetour.Add(new DefiService(defi, utilisateur, adversaire));
                }
            }
            return defisRetour;
        }

        /// <summary>
        /// Méthode permettant de récuperer le classement général
        /// </summary>
        /// <returns></returns>
        public List<UtilisateurService> RecupererClassement()
        {
            var classement = new List<UtilisateurService>();
            int cpt = 0;

            using (var db = new TaqnshareEntities())
            {
                var utilisateurs = from u in db.Utilisateurs
                                   orderby u.score_total_utilisateur / u.nombre_partie_utilisateur
                                   select u;

                foreach (Utilisateur u in utilisateurs)
                {
                    if (cpt < 10)
                        classement.Add(new UtilisateurService(u));
                    cpt++;
                }
            }
            return classement;
        }

        /// <summary>
        /// Méthode permettant de récupérer le rang du joueur
        /// </summary>
        /// <param name="idJoueur"></param>
        /// <returns></returns>
        public int RecupererRangJoueur(string idJoueur)
        {
            int rang = 0;
            int cpt = 1;
            using (var db = new TaqnshareEntities())
            {
                var utilisateurs = from u in db.Utilisateurs
                                   orderby u.score_total_utilisateur / u.nombre_partie_utilisateur
                                   select u;

                if (utilisateurs != null)
                    foreach (Utilisateur u in utilisateurs)
                    {
                        if (u.id_utilisateur == idJoueur)
                            rang = cpt;
                        cpt++;
                    }
            }
            return rang;
        }

        /// <summary>
        /// Méthode permettant de récupérer le score du joueur
        /// </summary>
        /// <param name="idJoueur"></param>
        /// <returns></returns>
        public float RecupererScoreJoueur(string idJoueur)
        {
            float score = 0;
            using (var db = new TaqnshareEntities())
            {
                var utilisateur = (from u in db.Utilisateurs
                                   where u.id_utilisateur == idJoueur
                                   select u).SingleOrDefault();

                if (utilisateur != null)
                    score = (float)(utilisateur.score_total_utilisateur / utilisateur.nombre_partie_utilisateur);
            }
            return score;
        }
    }
}
