using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Facebook;
using Facebook.Client;
using Facebook.Client.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TaqNShare.Resources;

namespace TaqNShare
{
    public partial class MainPage : PhoneApplicationPage
    {
        readonly CameraCaptureTask _camera;
        readonly PhotoChooserTask _galerie;

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            _camera = new CameraCaptureTask();
            _camera.Completed += new EventHandler<PhotoResult>(camera_Completed);

            _galerie = new PhotoChooserTask();
            _galerie.Completed += new EventHandler<PhotoResult>(galerie_Completed);
        }


        private void OnSessionStateChanged(object sender, SessionStateChangedEventArgs e)
        {
            this.ContentPanel.Visibility = (e.SessionState == FacebookSessionState.Opened)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void BoutonPrendrePhoto_Click(object sender, RoutedEventArgs e)
        {
            _camera.Show();
        }

        void camera_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Code to display the photo on the page in an image control named myImage.
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                //myImage.Source = bmp;
            }
        }

        void galerie_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MessageBox.Show(e.ChosenPhoto.Length.ToString());

                //Code to display the photo on the page in an image control named myImage.
                //System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                //bmp.SetSource(e.ChosenPhoto);
                //myImage.Source = bmp;
            }
        }
    }
}