using ServeurTaqnshare.ClasseDeService;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServeurTaqnshare
{
    /// <summary>
    /// Interface permettant d'exposer les méthodes du web service
    /// </summary>
    [ServiceContract(Name = "ServiceTaqnshare")]
    public interface IServiceTaqnshare
    {
        [OperationContract]
        bool EnregistrerScore(Utilisateur utilisateurCourant, int scorePartie);

        [OperationContract]
        bool CreerDefi(Defi partieUtilisateur, List<Composer> compositionTaquin, byte[] imageDefi);

        [OperationContract]
        DefiService RecupererDefi(int idDefi);

        [OperationContract]
        List<UtilisateurService> RecupererClassement();

        [OperationContract]
        int RecupererRangJoueur(string idJoueur);

        [OperationContract]
        float RecupererScoreJoueur(string idJoueur);

        [OperationContract]
        bool ModifierDefi(Defi defiTermine, Utilisateur utilisateurCourant);

        [OperationContract]
        List<DefiService> RecupererDefisEnAttente(string idUtilisateur);

        [OperationContract]
        List<DefiService> RecupererDefisUtilisateur(string idUtilisateur);

        [OperationContract]
        bool DeclinerDefi(int idDefiDecline);
    }

    
}

