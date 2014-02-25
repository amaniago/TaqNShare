using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace TaqNShare.Pages
{
    public partial class EnregistrerScorePage : PhoneApplicationPage
    {
        public EnregistrerScorePage()
        {
            InitializeComponent();
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