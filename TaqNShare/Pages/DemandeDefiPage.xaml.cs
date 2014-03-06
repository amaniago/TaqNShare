using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class DemandeDefiPage
    {
        readonly ServiceTaqnshareClient _serviceTaqnshareClient = new ServiceTaqnshareClient();
        readonly Partie _partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];

        public DemandeDefiPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _serviceTaqnshareClient.EnregistrerScoreCompleted += Enregistrement;
            _serviceTaqnshareClient.EnregistrerScoreAsync(App.UtilisateurCourant, _partieTermine.Score);
            base.OnNavigatedTo(e);
        }


        private void Enregistrement(object sender, EnregistrerScoreCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result);
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