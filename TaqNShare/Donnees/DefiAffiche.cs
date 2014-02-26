using System.Windows.Controls;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Donnees
{
    public class DefiAffiche
    {
        public int IdDefi { get; set; }
        public string NomDefi { get; set; }
        public string NomCreateur { get; set; }
        public string PrenomCreateur { get; set; }

        public DefiAffiche(DefiService defi)
        {
            IdDefi = defi.IdDefi;
            NomDefi = defi.NomDefi;
            NomCreateur = defi.NomUtilisateur;
            PrenomCreateur = defi.PrenomUtilisateur;
        }
    }
}
