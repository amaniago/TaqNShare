using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace TaqNShare.Views
{
    public partial class ValidationPhotoPage : PhoneApplicationPage
    {
        public ValidationPhotoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            BitmapImage imageSelectionne = (BitmapImage)PhoneApplicationService.Current.State["image"];
            ImageSelectionne.Source = imageSelectionne;
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            PhoneApplicationService.Current.State.Clear();
            base.OnBackKeyPress(e);
        }

        private void BoutonAccueil_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State.Clear();
            NavigationService.GoBack();
        }
    }
}