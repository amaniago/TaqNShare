using System;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class AfficherDetailDefiPage
    {
        public AfficherDetailDefiPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string idDefiQuery;
            int idDefi = 0;
            if (NavigationContext.QueryString.TryGetValue("idDefi", out idDefiQuery))
            {
                idDefi = Convert.ToInt32(idDefiQuery);
            }

            ServiceTaqnshareClient webServiceTaqnshareClient = new ServiceTaqnshareClient();

            webServiceTaqnshareClient.RecupererDefiCompleted += AfficherDefi;
            webServiceTaqnshareClient.RecupererDefiAsync(idDefi);
            base.OnNavigatedTo(e);
        }

        private void AfficherDefi(object sender, RecupererDefiCompletedEventArgs e)
        {
            DefiService defi = e.Result;
            PhoneApplicationService.Current.State["defi"] = defi;
            DefiImage.Source = new WriteableBitmap(Photo.DecodeImage(defi.ImageDefi));
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
    }
}