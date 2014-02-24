using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class EnregistrerScorePage : PhoneApplicationPage
    {
        readonly ServiceTaqnshareClient _webServiceTaqnshareClient = new ServiceTaqnshareClient();
        readonly Partie _partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];

        public EnregistrerScorePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Utilisateur utilisateur = new Utilisateur();
            utilisateur.id_utilisateur = App.IdFacebook;
            _webServiceTaqnshareClient.EnregistrerScoreCompleted += Enregistrement;
            _webServiceTaqnshareClient.EnregistrerScoreAsync(utilisateur, _partieTermine.Score);
            base.OnNavigatedTo(e);
        }

        private void Enregistrement(object sender, EnregistrerScoreCompletedEventArgs e)
        {
            MessageBox.Show(e.Result);
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