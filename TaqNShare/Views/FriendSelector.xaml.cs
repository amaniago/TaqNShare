using System;
using System.Windows;
using System.Collections.ObjectModel;
using TaqNShare.Data;

namespace TaqNShare.Views
{
    public partial class FriendSelector
    {
        public ObservableCollection<Amis> UtilisateurList { get; set; }

        public FriendSelector()
        {
            InitializeComponent();

            UtilisateurList = ListeAmis.Friends;

            DataContext = this;
        }

        private void retourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
        }
    }
}