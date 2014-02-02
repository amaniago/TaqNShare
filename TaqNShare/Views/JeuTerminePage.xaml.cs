using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.InteropServices.WindowsRuntime;
using TaqNShare.Data;

namespace TaqNShare.Views
{
    public partial class JeuTerminePage
    {
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
            Partie partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];
            TaquinTermineImage.Source = partieTermine.Photo;
            ScoreTextBlock.Text = "Votre score : " + partieTermine.Score + " Pts";
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
                NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}