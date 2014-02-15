using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TaqNShare.Data;
using Facebook;
using System.Windows;
using TaqNShare.TaqnshareReference;
using System.IO; 

namespace TaqNShare.Views
{
    public partial class MainPage
    {
        readonly CameraCaptureTask _camera;
        readonly PhotoChooserTask _galerie;

        public List<Parametre> Decoupages { get; set; }
        public List<Parametre> Filtres { get; set; }

        public int UserDecoupage { get; set; }
        public int UserFiltre { get; set; }

        ServiceTaqnshareClient s = new ServiceTaqnshareClient();

        /// <summary>
        /// Constructeur de la page
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            FacebookConnexion();

            DataContext = this;

            List<Parametre> p = new List<Parametre>
            {
                new Parametre(0, 9, 3),
                new Parametre(1, 16, 4),
                new Parametre(2, 25, 5)
            };

            Decoupages = p;

            int userDecoupage;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("IndexDecoupage", out userDecoupage);
            UserDecoupage = userDecoupage;


            Filtres = new List<Parametre>
            {
                new Parametre(0, 0, 0),
                new Parametre(1, 1, 0),
                new Parametre(2, 2, 0)
            };

            int userFiltre;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("IndexFiltre", out userFiltre);
            UserFiltre = userFiltre;

            _camera = new CameraCaptureTask();
            _camera.Completed += ChoixPhotoCompleted;

            _galerie = new PhotoChooserTask();
            _galerie.Completed += ChoixPhotoCompleted;

            //s.GetIdUtilisateurCompleted += RecupGetIdUtilisateur;
            //s.GetIdUtilisateurAsync();
        }

        /*public void RecupGetData(object sender, GetDataCompletedEventArgs e)
        {
            string test = e.Result;
            testText.Text = test;
        }*/

        public void RecupGetIdUtilisateur(object sender, GetIdUtilisateurCompletedEventArgs e)
        {
            string test = e.Result;
            testText.Text = test;
        }

        private void ButtonPrendrePhotoTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _camera.Show();
        }

        private void BoutonSelectPhotoTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _galerie.Show();
        }

        private void ChoixPhotoCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                WriteableBitmap imageSelectionne = BitmapFactory.New(1, 1).FromStream(e.ChosenPhoto);
                imageSelectionne = imageSelectionne.Resize(450, 732, WriteableBitmapExtensions.Interpolation.Bilinear);
                PhoneApplicationService.Current.State["photo"] = imageSelectionne;
                NavigationService.Navigate(new Uri("/Views/ValidationPhotoPage.xaml", UriKind.Relative));
            }
        }

        private void ListPickerDecoupageChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SaveSettings("TailleGrille", ListPickerDecoupage, false);
            SaveSettings("IndexDecoupage", ListPickerDecoupage, true);
        }

        private void ListPickerFiltreChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SaveSettings("IndexFiltre", ListPickerFiltre, true);
        }

        private static void SaveSettings(String key, ListPicker liste, bool casStock)
        {
            Parametre p = (Parametre)liste.SelectedItem;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains(key))
            {
                settings.Add(key, casStock ? p.Id : p.TailleGrille);
            }
            else
            {
                if (casStock)
                    settings[key] = p.Id;
                else
                    settings[key] = p.TailleGrille;

            }
            
            settings.Save();
        }

        private void ConnexionFacebookBouton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/FacebookLoginPage.xaml", UriKind.Relative));
        }

        private void FacebookConnexion()
        {
            if (App.isAuthenticated)
            {
                ConnexionFacebookBouton.Visibility = Visibility.Collapsed;
                DeConnexionFacebookBouton.Visibility = Visibility.Visible;
                MyName.Visibility = Visibility.Visible;
                MyImage.Visibility = Visibility.Visible;
                LoadUserInfo();
            }
            else
            {
                ConnexionFacebookBouton.Visibility = Visibility.Visible;
                DeConnexionFacebookBouton.Visibility = Visibility.Collapsed;
                MyName.Visibility = Visibility.Collapsed;
                MyImage.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadUserInfo()
        {
            var fb = new FacebookClient(App.AccessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                Dispatcher.BeginInvoke(() =>
                {
                    var profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", App.FacebookId, "square", App.AccessToken);

                    MyImage.Source = new BitmapImage(new Uri(profilePictureUrl));
                    if (MyName != null)
                        MyName.Text = String.Format("{0} {1}", result["first_name"], result["last_name"]);
                });
            };
            fb.GetTaskAsync("me");
        }

        private async void DeConnexionFacebookBouton_Click(object sender, RoutedEventArgs e)
        {
            App.isAuthenticated = false;

            WebBrowser FacebookWebBrowser = new WebBrowser();

            FacebookWebBrowser.Navigate(new Uri(String.Format("https://www.facebook.com/logout.php?next={0}&access_token={1}", "http://www.facebook.com", App.AccessToken)));
            await FacebookWebBrowser.ClearCookiesAsync();
            FacebookConnexion();

        }

        private string tests;
        private string nomImage = "TaqNShare";
        private byte[] ImageArray2;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //var profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", App.FacebookId, "square", App.AccessToken);

            //imageTest.Source = new BitmapImage(new Uri("/TaqNShare.png", UriKind.Relative));

            byte[] ImageArray = new byte[] { 0 };
            
            
            BitmapImage ima = new BitmapImage(new Uri("/TaqNShare.png", UriKind.Relative));
           
            ImageArray = ConvertToBytes(ima);

            //s.UploadFileCompleted += testUploadImage;
            //s.UploadFileAsync(ImageArray, nomImage);

            s.GetCoucouCompleted += recupCoucou;
            s.GetCoucouAsync(ImageArray, nomImage);
           
      

            MessageBox.Show("coucou");
            //MessageBox.Show(tests);

            s.GetImageFileCompleted += recupImage;
            s.GetImageFileAsync(nomImage);
           
            //imageTest.Source = decodeImage(ImageArray2);
        }

        public static byte[] ConvertToBytes(BitmapImage bitmapImage)
        {
            BitmapImage image = new BitmapImage();
            image.CreateOptions = BitmapCreateOptions.None;
            image.UriSource = new Uri("/TaqNShare.png", UriKind.Relative);
            WriteableBitmap wbmp = new WriteableBitmap(image);
            MemoryStream ms = new MemoryStream();
            wbmp.SaveJpeg(ms, wbmp.PixelWidth, wbmp.PixelHeight, 0, 100);
            return ms.ToArray();

            //return data;
        }

        public BitmapImage decodeImage(byte[] array)
        {
            Stream stream = new MemoryStream(array);
            BitmapImage image = new BitmapImage();

            //image.;
            //MessageBox.Show(array);
            image.SetSource(stream);

            return image;
        } 

        public void testUploadImage(object sender, UploadFileCompletedEventArgs e)
        {
            string test = e.Result;
            testText.Text = test;
        }

        public void recupImage(object senfer, GetImageFileCompletedEventArgs e)
        {
            byte[] b = e.Result;
            ImageArray2 = b;
            //string b = e.Result;
        }

        public void recupCoucou(object sender, GetCoucouCompletedEventArgs e)
        {
            string coucou = e.Result;
            testText.Text = coucou;
        }
    }
}