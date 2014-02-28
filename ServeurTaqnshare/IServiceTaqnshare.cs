using ServeurTaqnshare.ClasseDeService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace ServeurTaqnshare
{
    [ServiceContract(Name = "ServiceTaqnshare")]
    public interface IServiceTaqnshare
    {
        [OperationContract]
        string EnvoyerImage(byte[] imageByte, string nomImage);

        [OperationContract]
        byte[] RecupererImage(string nomImage);

        [OperationContract]
        bool EnregistrerScore(Utilisateur utilisateurCourant, int scorePartie);

        [OperationContract]
        string CreerDefi(Defi partieUtilisateur, List<Composer> compositionTaquin);

        [OperationContract]
        List<UtilisateurService> RecupererClassement();

        [OperationContract]
        int RecupererRangJoueur(string idJoueur);

        [OperationContract]
        float RecupererScoreJoueur(string idJoueur);
    }

    public class ServiceTaqnshare : IServiceTaqnshare
    {
        public string EnvoyerImage(byte[] imageByte, string nomImage)
        {
            try
            {
                MemoryStream ms = new MemoryStream(imageByte);
                //FileStream fs = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName, FileMode.Create);
                FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + nomImage,
                                               FileMode.Create);

                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();
                return "OK!";
            }
            catch (Exception ex)
            {
                return (ex.ToString());
            }
        }

        public byte[] RecupererImage(string nomImage)
        {

            //return File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName);
            return File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + nomImage);

        }

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
                using (var db = new TaqnshareEntities())
                {
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
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string CreerDefi(Defi partieUtilisateur, List<Composer> compositionTaquin)
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

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
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

