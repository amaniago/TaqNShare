using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class EnregistrerScorePage
    {
        readonly ServiceTaqnshareClient _webService = new ServiceTaqnshareClient();
        readonly Partie _partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];

        public EnregistrerScorePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Utilisateur utilisateur = new Utilisateur {id_utilisateur = App.IdFacebook};
            _webService.EnregistrerScoreCompleted += Enregistrement;
            _webService.EnregistrerScoreAsync(utilisateur, _partieTermine.Score); 
        }

        private void Enregistrement(object sender, EnregistrerScoreCompletedEventArgs e)
        {
            bool estEnregistre;   
            estEnregistre = e.Result;
            if (!estEnregistre)
            {
                MessageBox.Show("Une erreur s'est produite lors de l'enregistrement !");
                //NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            }
        }

        private void RetourAccueilBoutonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sur de vouloir retourner à l'accueil ? " +
                            "Si vous acceptez, votre score sera perdu !", " ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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