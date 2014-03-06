using System;
using System.Runtime.Serialization;

namespace ServeurTaqnshare.ClasseDeService
{
    /// <summary>
    /// Classe permettant de transmettre les infos d'un utilisateur par le web service
    /// </summary>
    [DataContract]
    public class UtilisateurService
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="utilisateur"></param>
        public UtilisateurService(Utilisateur utilisateur)
        {
            IdUtilisateur = utilisateur.id_utilisateur;
            NomUtilisateur = utilisateur.nom_utilisateur;
            PrenomUtilisateur = utilisateur.prenom_utilisateur;
            NombrePartieUtilisateur = Convert.ToInt32(utilisateur.nombre_partie_utilisateur);
            ScoreTotalUtilisateur = Convert.ToInt32(utilisateur.score_total_utilisateur);
        }

        [DataMember]
        public string IdUtilisateur { get; set; }

        [DataMember]
        public string NomUtilisateur { get; set; }

        [DataMember]
        public string PrenomUtilisateur { get; set; }

        [DataMember]
        public decimal NombrePartieUtilisateur { get; set; }

        [DataMember]
        public decimal ScoreTotalUtilisateur { get; set; }
    }
}
