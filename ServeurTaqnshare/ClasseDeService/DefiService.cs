using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServeurTaqnshare.ClasseDeService
{
    /// <summary>
    /// Classe permettant de retourner un defi, son image et sa composition à travers le web service
    /// </summary>
    [DataContract(IsReference = true)]
    public sealed class DefiService
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur utilisé dans le cas de la récupération d'un défi pour pouvoir le résoudre
        /// </summary>
        /// <param name="defi"></param>
        /// <param name="imageDefi"></param>
        /// <param name="nombreFiltre"></param>
        /// <param name="utilisateur"></param>
        public DefiService(Defi defi, byte[] imageDefi, int nombreFiltre, Utilisateur utilisateur)
        {
            IdDefi = defi.id_defi;
            IdUtilisateur = defi.id_utilisateur;
            IdAdversaireDefi = defi.id_adversaire_defi;
            NomDefi = defi.nom_defi;
            ScoreUtilisateurDefi = Convert.ToInt32(defi.score_utilisateur_defi);
            ImageDefi = imageDefi;
            NombreFiltre = nombreFiltre;
            NomUtilisateur = utilisateur.nom_utilisateur;
            PrenomUtilisateur = utilisateur.prenom_utilisateur;

            Composition = new List<CompositionService>();

            foreach (var composer in defi.Composers)
            {
                Composition.Add(new CompositionService(composer));
            }
        }

        /// <summary>
        /// Constructeur utilisé dans le cas de la récupération des defis en attente de l'utilisateur
        /// </summary>
        /// <param name="defi"></param>
        /// <param name="createurDefi"></param>
        public DefiService(Defi defi, Utilisateur createurDefi)
        {
            IdDefi = defi.id_defi;
            NomDefi = defi.nom_defi;
            NomUtilisateur = createurDefi.nom_utilisateur;
            PrenomUtilisateur = createurDefi.prenom_utilisateur;
        }

        /// <summary>
        /// Constructeur utilisé dans le cas de la récupération des défis terminés de l'utilisateur
        /// </summary>
        /// <param name="defi"></param>
        /// <param name="utilisateur"></param>
        /// <param name="adversaire"></param>
        public DefiService(Defi defi, Utilisateur utilisateur, Utilisateur adversaire)
        {
            NomDefi = defi.nom_defi;
            PrenomUtilisateur = utilisateur.prenom_utilisateur;
            NomUtilisateur = utilisateur.nom_utilisateur;
            PrenomAdversaire = adversaire.prenom_utilisateur;
            NomAdversaire = adversaire.nom_utilisateur;
            ScoreUtilisateurDefi = (int)defi.score_utilisateur_defi;
            ScoreAdversaireDefi = (int)defi.score_adversaire_defi;
        }

        #endregion Constructeurs

        #region Propriétés
        [DataMember]
        public int IdDefi { get; set; }
        [DataMember]
        public byte[] ImageDefi { get; set; }
        [DataMember]
        public string IdUtilisateur { get; set; }
        [DataMember]
        public string IdAdversaireDefi { get; set; }
        [DataMember]
        public string NomDefi { get; set; }
        [DataMember]
        public int ScoreUtilisateurDefi { get; set; }
        [DataMember]
        public int ScoreAdversaireDefi { get; set; }
        [DataMember]
        public List<CompositionService> Composition { get; set; }
        [DataMember]
        public string NomUtilisateur { get; set; }
        [DataMember]
        public string PrenomUtilisateur { get; set; }
        [DataMember]
        public int NombreFiltre { get; set; }
        [DataMember]
        public string NomAdversaire { get; set; }
        [DataMember]
        public string PrenomAdversaire { get; set; }

        #endregion Propriétés
    }
        
    
}
