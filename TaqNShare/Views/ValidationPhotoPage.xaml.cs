using System;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;

namespace TaqNShare.Views
{
    /// <summary>
    /// Page permettant la validation de la photo prise :
    ///     - Si ok passage a la page de Jeu
    ///     - Si non retour accueil
    /// </summary>
    public partial class ValidationPhotoPage
    {
        public ValidationPhotoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            WriteableBitmap photo = (WriteableBitmap)PhoneApplicationService.Current.State["photo"];
            ImageSelectionne.Source = photo;
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            PhoneApplicationService.Current.State.Clear();
            base.OnBackKeyPress(e);
        }

        private void BoutonAccueilTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State.Clear();
            NavigationService.GoBack();
        }

        private void BoutonLancerJeuTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/JeuPage.xaml", UriKind.Relative));
        }
    }
}