using System.Collections.ObjectModel;

namespace TaqNShare.Data
{
    public class ListeAmis
    {
        private static readonly ObservableCollection<Amis> friends = new ObservableCollection<Amis>();

        public static ObservableCollection<Amis> Friends
        {
            get
            {
                return friends;
            }
        }

        private static readonly ObservableCollection<Amis> selectedFriends = new ObservableCollection<Amis>();

        public static ObservableCollection<Amis> SelectedFriends
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
