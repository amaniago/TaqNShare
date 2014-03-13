using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class AfficherDetailDefiPage
    {
        private readonly ServiceTaqnshareClient _webServiceTaqnshareClient = new ServiceTaqnshareClient();
        private DefiService _defi;

        public AfficherDetailDefiPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AccepterDefiBouton.IsEnabled = false;
            RetourAccueilBouton.IsEnabled = false;
            DeclinerDefiBouton.IsEnabled = false;
            string idDefiQuery;
            int idDefi = 0;
            if (NavigationContext.QueryString.TryGetValue("idDefi", out idDefiQuery))
            {
                idDefi = Convert.ToInt32(idDefiQuery);
            }

            _webServiceTaqnshareClient.RecupererDefiCompleted += AfficherDefi;
            _webServiceTaqnshareClient.RecupererDefiAsync(idDefi);
            base.OnNavigatedTo(e);
        }

        private void AfficherDefi(object sender, RecupererDefiCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                AccepterDefiBouton.IsEnabled = true;
                RetourAccueilBouton.IsEnabled = true;
                DeclinerDefiBouton.IsEnabled = true;
                _defi = e.Result;
                PhoneApplicationService.Current.State["defi"] = _defi;
                DefiImage.Source = new WriteableBitmap(Photo.DecodeImage(_defi.ImageDefi));
                CreateurDefiTextBlock.Text = "Votre ami " + _defi.PrenomUtilisateur + " " + _defi.NomUtilisateur +
                                             " vous a défié !";
                NombreDecoupageDefiTextBlock.Text = "Découpages : " + (_defi.Composition.Count + 1);
                NombreFiltreDefiTextBlock.Text = "Filtres : " + _defi.NombreFiltre;
            }
            else
            {
                MessageBox.Show("Une erreur s'est produite lors de la récupération du défi!");
                RetourAccueilBouton.IsEnabled = true;
            }

        }

        private void AccepterDefiBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/JeuPage.xaml?casDefi=" + true, UriKind.Relative));
        }

        private void RetourAccueilBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State.Clear();
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        private void DeclinerDefiBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _webServiceTaqnshareClient.DeclinerDefiCompleted += DelinerDefi;
            _webServiceTaqnshareClient.DeclinerDefiAsync(_defi.IdDefi);
        }

        private void DelinerDefi(object sender, DeclinerDefiCompletedEventArgs e)
        {
            if (e.Result)
            {
                PhoneApplicationService.Current.State.Clear();
                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Une erreur s'est produite lors de la déclinaison du défi!");
                RetourAccueilBouton.IsEnabled = true;
            }
        }
    }
}