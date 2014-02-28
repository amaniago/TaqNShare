using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServeurTaqnshare.ClasseDeService
{
    [DataContract]
    public class UtilisateurService
    {
        public UtilisateurService(Utilisateur u)
        {
            IdUtilisateur = u.id_utilisateur;
            NomUtilisateur = u.nom_utilisateur;
            PrenomUtilisateur = u.prenom_utilisateur;
            NombrePartieUtilisateur = Convert.ToInt32(u.nombre_partie_utilisateur);
            ScoreTotalUtilisateur = Convert.ToInt32(u.score_total_utilisateur);
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
