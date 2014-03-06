using System;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class JeuTerminePage
    {
        readonly ServiceTaqnshareClient _serviceTaqnshareClient = new ServiceTaqnshareClient();
        readonly Partie _partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];

        public JeuTerminePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Méthode appelée lors de la navigation entre les pages
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Récupération de la partie de l'utilisateur
            TaquinTermineImage.Source = _partieTermine.Photo;
            ScoreTextBlock.Text = "Votre score : " + _partieTermine.Score + " Pts";
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            PhoneApplicationService.Current.State.Clear();
            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// Méthode pour enregistrer le score en base
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnregistrementScoreBoutonClick(object sender, RoutedEventArgs e)
        {
            if (App.EstAuthentifie)
            {
                EnregistrementScoreBouton.IsEnabled = false;
                RetourAccueilBouton.IsEnabled = false;
                _serviceTaqnshareClient.EnregistrerScoreCompleted += Enregistrement;
                _serviceTaqnshareClient.EnregistrerScoreAsync(App.UtilisateurCourant, _partieTermine.Score);
            }
            else
            {
                if (MessageBox.Show("Pour enregistrer une partie vous devez être connecté à Facebook. Voulez-vous vous connecter?", " ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/Pages/AuthentificationFacebookPage.xaml?pageAvant=DemandeDefiPage", UriKind.Relative));
                }
            }
        }

        private void Enregistrement(object sender, EnregistrerScoreCompletedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DemandeDefiPage.xaml", UriKind.Relative));
        }
        
        /// <summary>
        /// Méthode permettant le retour à l'accueil sans enregistrement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetourAccueilBoutonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sur de vouloir retourner à l'accueil ? " +
                            "Si vous acceptez, votre score sera perdu !", " ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PhoneApplicationService.Current.State.Clear();
                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}