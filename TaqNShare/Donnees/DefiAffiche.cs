using TaqNShare.TaqnshareReference;

namespace TaqNShare.Donnees
{
    /// <summary>
    /// Permet de remplir la liste des defis a afficher
    /// </summary>
    public class DefiAffiche
    {
        #region Propriétés
        public int IdDefi { get; set; }
        public string NomDefi { get; set; }
        public string NomCreateur { get; set; }
        public string PrenomCreateur { get; set; }
        #endregion

        public DefiAffiche(DefiService defi)
        {
            IdDefi = defi.IdDefi;
            NomDefi = defi.NomDefi;
            NomCreateur = defi.NomUtilisateur;
            PrenomCreateur = defi.PrenomUtilisateur;
        }
    }
}
