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
            _defi = e.Result;
            PhoneApplicationService.Current.State["defi"] = _defi;
            DefiImage.Source = new WriteableBitmap(Photo.DecodeImage(_defi.ImageDefi));
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
            MessageBox.Show(e.Result);
        }
    }
}