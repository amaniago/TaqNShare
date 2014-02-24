    using System.Collections.ObjectModel;

namespace TaqNShare.Donnees
{
    public class ListeAmis
    {
        private static readonly ObservableCollection<UtilisateurFacebook> amis = new ObservableCollection<UtilisateurFacebook>();

        public static ObservableCollection<UtilisateurFacebook> Amis
        {
            get
            {
                return amis;
            }

            
        }

        public void Ajouter(UtilisateurFacebook u)
        {
            amis.Add(u);
        }

        private static readonly ObservableCollection<UtilisateurFacebook> amisSelectionnes = new ObservableCollection<UtilisateurFacebook>();

        public static ObservableCollection<UtilisateurFacebook> AmisSelectionnes
        {
            get
            {
                return amisSelectionnes;
            }
        }

        public static void Vider()
        {
            amisSelectionnes .Clear();
        }
    }
}
