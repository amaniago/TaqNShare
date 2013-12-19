using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TaqNShare.Data;

namespace TaqNShare.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        readonly CameraCaptureTask _camera;
        readonly PhotoChooserTask _galerie;

        public List<Parametre> Decoupages { get; set; }
        public List<Parametre> Filtres { get; set; }

        public int UserDecoupage { get; set; }
        public int UserFiltre { get; set; }

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;

            List<Parametre> p = new List<Parametre>
            {
                new Parametre(0, 9),
                new Parametre(1, 16),
                new Parametre(2, 25)
            };

            Decoupages = p;

            int userDecoupage = 0;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("Decoupage", out userDecoupage);
            UserDecoupage = userDecoupage;


            Filtres = new List<Parametre>
            {
                new Parametre(0, 1),
                new Parametre(1, 2),
                new Parametre(2, 4)
            };

            int userFiltre = 0;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("Filtre", out userFiltre);
            UserFiltre = userFiltre;

            _camera = new CameraCaptureTask();
            _camera.Completed += new EventHandler<PhotoResult>(choixPhoto_Completed);

            _galerie = new PhotoChooserTask();
            _galerie.Completed += new EventHandler<PhotoResult>(choixPhoto_Completed);
        }

        private void ButtonPrendrePhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _camera.Show();
        }

        private void BoutonSelectPhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _galerie.Show();
        }

        void choixPhoto_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {          
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                PhoneApplicationService.Current.State["image"] = bmp;
                NavigationService.Navigate(new Uri("/Views/ValidationPhotoPage.xaml", UriKind.Relative));
            }
        }

        private void ListPickerDecoupage_change(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SaveSettings("Decoupage", ListPickerDecoupage);
        }

        private void ListPickerFiltre_change(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SaveSettings("Filtre", ListPickerFiltre);
        }

        private void SaveSettings(String key, ListPicker liste)
        {
            Parametre p = (Parametre)liste.SelectedItem;
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (!settings.Contains(key))
            {
                settings.Add(key, p.Id);
            }
            else
            {
                settings[key] = p.Id;
            }
            settings.Save();
        }
        
    }
}