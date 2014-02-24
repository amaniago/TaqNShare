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
        string UploadFile(byte[] f, string fileName);

        [OperationContract]
        byte[] GetImageFile(string fileName);

        [OperationContract]
        bool EnregistrerScore(Utilisateur utilisateurCourant, int scorePartie);

        [OperationContract]
        string CreerDefi(Defi partieUtilisateur, List<Composer> compositionTaquin);
    }

    public class ServiceTaqnshare : IServiceTaqnshare
    {

        public string UploadFile(byte[] f, string fileName)
        {
            try
            {
                MemoryStream ms = new MemoryStream(f);
                //FileStream fs = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName, FileMode.Create);
                FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + fileName, FileMode.Create);


                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();
                return "OK";
            }
            catch (Exception ex)
            {
                return (ex.ToString());
            }
            //return Directory.GetCurrentDirectory();
        }

        public byte[] GetImageFile(string fileName)
        {
            //return System.IO.File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + fileName);

            return System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\ImagesWebService\\" + fileName);


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
    }
}

