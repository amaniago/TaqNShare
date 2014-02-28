﻿using ServeurTaqnshare.ClasseDeService;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.ServiceModel;
using ServeurTaqnshare.ClasseDeService;

namespace ServeurTaqnshare
{
    /// <summary>
    /// Interface permettant d'exposer les méthodes du web service
    /// </summary>
    [ServiceContract(Name = "ServiceTaqnshare")]
    public interface IServiceTaqnshare
    {
        [OperationContract]
        string EnregistrerScore(Utilisateur utilisateurCourant, int scorePartie);

        [OperationContract]
        string CreerDefi(Defi partieUtilisateur, List<Composer> compositionTaquin, byte[] imageDefi);

        [OperationContract]
        DefiService RecupererDefi(int idDefi);

        [OperationContract]
        List<UtilisateurService> RecupererClassement();

        [OperationContract]
        int RecupererRangJoueur(string idJoueur);

        [OperationContract]
        float RecupererScoreJoueur(string idJoueur);

        [OperationContract]
        string ModifierDefi(Defi defiTermine);

        [OperationContract]
        List<DefiService> RecupererDefis(string idUtilisateur);
    }

    public class ServiceTaqnshare : IServiceTaqnshare
    {
        /// <summary>
        /// Méthode permettant d'enregistrer le score de l'utilisateur
        /// </summary>
        /// <param name="utilisateurCourant"></param>
        /// <param name="scorePartie"></param>
        /// <returns></returns>
        public string EnregistrerScore(Utilisateur utilisateurCourant, int scorePartie)
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
                                                            nombre_partie_utilisateur = 1,
                                                            score_total_utilisateur = scorePartie
                                                        };
                        db.Utilisateurs.Add(nouvelUtilisateur);
                    }
                    db.SaveChanges();

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Méthode permettant de créer un défi et de stocker l'image associer
        /// </summary>
        /// <param name="partieUtilisateur"></param>
        /// <param name="compositionTaquin"></param>
        /// <param name="imageDefi"></param>
        /// <returns></returns>
        public string CreerDefi(Defi partieUtilisateur, List<Composer> compositionTaquin, byte[] imageDefi)
        {
            try
            {

                var db = new TaqnshareEntities();

                var nouveauDefi = db.Defis.Add(partieUtilisateur);
                db.SaveChanges();


                foreach (Composer composer in compositionTaquin)
                {
                    composer.id_defi = nouveauDefi.id_defi;
                }

                db.Composers.AddRange(compositionTaquin);
                db.SaveChanges();

                MemoryStream ms = new MemoryStream(imageDefi);
                FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\ImagesWebService\\Image" + nouveauDefi.id_defi + ".jpg", FileMode.Create);

                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();

                return "OK";
            }
            catch (Exception e)
            {
                return e.InnerException.ToString();
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

            byte[] imageDefi = File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\ImagesWebService\\Image" + idDefi + ".jpg");

            DefiService retour = new DefiService(defi, imageDefi);

            return retour;
        }

        /// <summary>
        /// Méthode permettant de modifier un défi lorsque celui-ci a été accepter par l'adversaire
        /// </summary>
        /// <param name="defiTermine"></param>
        /// <returns></returns>
        public string ModifierDefi(Defi defiTermine)
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

                return "OK";
            }
            catch (Exception e)
            {
                return e.InnerException.ToString();
            }
        }

        public List<DefiService> RecupererDefis(string idUtilisateur)
        {

            var db = new TaqnshareEntities();

            List<Defi> defis = (from d in db.Defis
                                where d.id_adversaire_defi == idUtilisateur
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

        public List<UtilisateurService> RecupererClassement()
        {
            var classement = new List<UtilisateurService>();
            int cpt = 0;

            using (var db = new TaqnshareEntities())
            {
                var utilisateurs = from u in db.Utilisateurs
                                    orderby u.score_total_utilisateur/u.nombre_partie_utilisateur 
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

        public int RecupererRangJoueur (string idJoueur)
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

        public float RecupererScoreJoueur (string idJoueur)
        {
            float score = 0;
            using (var db = new TaqnshareEntities())
            {
                var utilisateur = (from u in db.Utilisateurs
                                   where u.id_utilisateur == idJoueur
                                   select u).SingleOrDefault();

                if (utilisateur != null)
                    score = (float) (utilisateur.score_total_utilisateur/utilisateur.nombre_partie_utilisateur);
            }

            return score;

            

        }
    }


}

