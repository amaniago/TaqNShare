using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class DefiTerminePage
    {
        public DefiTerminePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RetourAccueilBouton.IsEnabled = false;
            //Récupération de la photo
            DefiService defi = (DefiService)PhoneApplicationService.Current.State["defi"];
            TaquinTermineImage.Source = new WriteableBitmap(Photo.DecodeImage(defi.ImageDefi));

            //Récupération du score
            Partie partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];
            ScoreTextBlock.Text = "Votre score : " + partieTermine.Score + " Pts";

            Defi defiTermine = new Defi();
            defiTermine.id_defi = defi.IdDefi;
            defiTermine.score_adversaire_defi = partieTermine.Score;
            defiTermine.resolu = true;

            ServiceTaqnshareClient serviceTaqnshareClient = new ServiceTaqnshareClient();
            serviceTaqnshareClient.ModifierDefiCompleted += DefiModifie;
            serviceTaqnshareClient.ModifierDefiAsync(defiTermine, App.UtilisateurCourant);
           
            base.OnNavigatedTo(e);
        }

        private void DefiModifie(object sender, ModifierDefiCompletedEventArgs e)
        {
            if (e.Result)
            {
                RetourAccueilBouton.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Une erreur s'est produite lors de l'enregistrement!");
                RetourAccueilBouton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Méthode permettant le retour à l'accueil
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetourAccueilBoutonTap(object sender, GestureEventArgs e)
        {
            PhoneApplicationService.Current.State.Clear();
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }
    }
}