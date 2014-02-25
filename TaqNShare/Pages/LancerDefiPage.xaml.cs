using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class LancerDefiPage
    {
       
        public LancerDefiPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ServiceTaqnshareClient webServiceTaqnshareClient = new ServiceTaqnshareClient();
            
            webServiceTaqnshareClient.RecupererDefiCompleted += AfficherDefi;
            webServiceTaqnshareClient.RecupererDefiAsync(13);
            base.OnNavigatedTo(e);
        }

        private void AfficherDefi(object sender, RecupererDefiCompletedEventArgs e)
        {
            List<object> retours = e.Result;

            //Defi defi = retours[0] as Defi;
            WriteableBitmap imageDefi = new WriteableBitmap(Photo.DecodeImage(retours[1] as byte[]));

            DefiImage.Source = imageDefi;
        }

        private void AccepterDefiBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void RetourAccueilBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }


    }
}