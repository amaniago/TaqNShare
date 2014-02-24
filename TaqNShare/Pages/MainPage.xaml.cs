using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TaqNShare.Donnees;
using Facebook;
using System.Windows;
using System.Windows.Navigation;
using TaqNShare.TaqnshareReference;



namespace TaqNShare.Pages
{
    public partial class MainPage
    {
        readonly CameraCaptureTask _camera;
        readonly PhotoChooserTask _galerie;

        public List<Parametre> Decoupages { get; set; }
        public List<Parametre> Filtres { get; set; }

        public int UserDecoupage { get; set; }
        public int UserFiltre { get; set; }

        /// <summary>
        /// Constructeur de la page
        /// </summary>
        public MainPage()
        {
            bool utilisateurConnecte = false;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("UtilisateurConnecte", out utilisateurConnecte);

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

            if (!App.EstAuthentifie && utilisateurConnecte)
                Loaded += ConnexionFacebookBoutonClick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            FacebookConnexion();
            base.OnNavigatedTo(e);
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
                NavigationService.Navigate(new Uri("/Pages/ValidationPhotoPage.xaml", UriKind.Relative));
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

        private void ConnexionFacebookBoutonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AuthentificationFacebookPage.xaml?pageAvant=MainPage", UriKind.Relative));
        }

        private void FacebookConnexion()
        {
            if (App.EstAuthentifie)
            {
                ConnexionFacebookBouton.Visibility = Visibility.Collapsed;
                DeConnexionFacebookBouton.Visibility = Visibility.Visible;
                nom.Visibility = Visibility.Visible;
                photo.Visibility = Visibility.Visible;
                LoadUserInfo();
            }
            else
            {
                ConnexionFacebookBouton.Visibility = Visibility.Visible;
                DeConnexionFacebookBouton.Visibility = Visibility.Collapsed;
                nom.Visibility = Visibility.Collapsed;
                photo.Visibility = Visibility.Collapsed;
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
                    var profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", App.IdFacebook, "square", App.AccessToken);

                    photo.Source = new BitmapImage(new Uri(profilePictureUrl));
                    if (nom != null)
                        nom.Text = String.Format("{0} {1}", result["first_name"], result["last_name"]);
                });
            };
            fb.GetTaskAsync("me");
        }

        private async void DeConnexionFacebookBoutonClick(object sender, RoutedEventArgs e)
        {
            App.EstAuthentifie = false;

            var facebookWebBrowser = new WebBrowser();

            facebookWebBrowser.Navigate(new Uri(String.Format("https://www.facebook.com/logout.php?next={0}&access_token={1}", "http://www.facebook.com", App.AccessToken)));
            await facebookWebBrowser.ClearCookiesAsync();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            const string key = "UtilisateurConnecte";

            if (!settings.Contains(key))
                settings.Add(key, false);
            else
                settings[key] = false;


            settings.Save();

            FacebookConnexion();
        }

        /*
        ServiceTaqnshareClient service = new ServiceTaqnshareClient();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.CreateOptions = BitmapCreateOptions.None;
            image.UriSource = new Uri("/TaqNShare.png", UriKind.Relative);
            WriteableBitmap wbmp = new WriteableBitmap(image);

            byte[] test = ConvertToBytes(wbmp);

            this.service.EnvoyerImageCompleted += EnvoyerImage;
            this.service.EnvoyerImageAsync(test, "TaqNShare.png");
        }

        private void EnvoyerImage(object sender, EnvoyerImageCompletedEventArgs e)
        {
            MessageBox.Show(e.Result);
            service.RecupererImageCompleted += AfficherImage;
            service.RecupererImageAsync("TaqNShare.png");
        }

        private void AfficherImage(object sender, RecupererImageCompletedEventArgs e)
        {
            testImage.Source = decodeImage(e.Result);
        }

        public static byte[] ConvertToBytes(WriteableBitmap wbmp)
        {
            MemoryStream ms = new MemoryStream();
            wbmp.SaveJpeg(ms, wbmp.PixelWidth, wbmp.PixelHeight, 0, 100);
            return ms.ToArray();
        }

        public BitmapImage decodeImage(byte[] array)
        {
            Stream stream = new MemoryStream(array);
            BitmapImage image = new BitmapImage();

            image.SetSource(stream);

            return image;
        } 
         * */
    }
}