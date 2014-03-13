using System;
using System.Windows;
using Microsoft.Phone.Shell;

namespace TaqNShare.Pages
{
    /// <summary>
    /// Classe permettant de valider le choix de l'utilisateur de défier ou non un ami
    /// </summary>
    public partial class DemandeDefiPage
    {
        public DemandeDefiPage()
        {
            InitializeComponent();
        }

        private void RetourAccueilBoutonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sur de vouloir retourner à l'accueil ?", " ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PhoneApplicationService.Current.State.Clear();
                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            }
        }

        private void DefierAmiBoutonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DefierAmiPage.xaml", UriKind.Relative));
        }
    }
}