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
        public List<Composition> Composition { get; set; }
        [DataMember]
        public string NomUtilisateur { get; set; }
        [DataMember]
        public string PrenomUtilisateur { get; set; }

        public DefiService(Defi defi, byte[] imageDefi)
        {
            IdDefi = defi.id_defi;
            IdUtilisateur = defi.id_utilisateur;
            IdAdversaireDefi = defi.id_adversaire_defi;
            NomDefi = defi.nom_defi;
            ScoreUtilisateurDefi = Convert.ToInt32(defi.score_utilisateur_defi);
            ImageDefi = imageDefi;

            Composition = new List<Composition>();

            foreach (var composer in defi.Composers)
            {
                Composition.Add(new Composition(composer));
            }
        }

        public DefiService(Defi defi, Utilisateur createurDefi)
        {
            IdDefi = defi.id_defi;
            NomDefi = defi.nom_defi;
            NomUtilisateur = createurDefi.nom_utilisateur;
            PrenomUtilisateur = createurDefi.prenom_utilisateur;
        }

    }

    [DataContract(IsReference = true)]
    public class Composition
    {
        public Composition(Composer composer)
        {
            IdPiece = composer.id_piece;
            IdFiltre = composer.id_filtre;
            IndexPosition = Convert.ToInt32(composer.position_piece);
        }

        [DataMember]
        public int IdPiece { get; set; }
        [DataMember]
        public int IdFiltre { get; set; }
        [DataMember]
        public int IndexPosition { get; set; }
    }
}
