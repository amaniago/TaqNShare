using System.Collections.ObjectModel;

namespace TaqNShare.Data
{
    public class ListeAmis
    {
        private static readonly ObservableCollection<UtilisateurFacebook> friends = new ObservableCollection<UtilisateurFacebook>();

        public static ObservableCollection<UtilisateurFacebook> Friends
        {
            get
            {
                return friends;
            }
        }

        private static readonly ObservableCollection<UtilisateurFacebook> selectedFriends = new ObservableCollection<UtilisateurFacebook>();

        public static ObservableCollection<UtilisateurFacebook> SelectedFriends
        {
            get
            {
                return selectedFriends;
            }
        }

        public static void Vider()
        {
            selectedFriends .Clear();
        }
    }
}
