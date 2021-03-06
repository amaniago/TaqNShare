﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TaqNShare.Donnees;
using System.Windows;
using System.Windows.Navigation;
using TaqNShare.TaqnshareReference;


namespace TaqNShare.Pages
{
    /// <summary>
    /// Page principale : contient le panorama d'accueil avec ses quatres items.
    /// </summary>
    public partial class MainPage
    {
        #region propriétés

        private ServiceTaqnshareClient _serviceTaqnshareClient = new ServiceTaqnshareClient();
        private readonly ObservableCollection<Classement> _classement = new ObservableCollection<Classement>();

        private readonly CameraCaptureTask _camera;
        private readonly PhotoChooserTask _galerie;

        public List<Parametre> Decoupages { get; set; }
        public List<Parametre> Filtres { get; set; }

        public int UserDecoupage { get; set; }
        public int UserFiltre { get; set; }

        private ObservableCollection<DefiAffiche> _defisAAfficher = new ObservableCollection<DefiAffiche>();

        #endregion propriétés

        /// <summary>
        /// Constructeur de la page
        /// </summary>
        public MainPage()
        {
            bool utilisateurConnecte;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("UtilisateurConnecte", out utilisateurConnecte);

            InitializeComponent();


            AffichageRangScore();

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

            //Récupération du classement
            _serviceTaqnshareClient.RecupererClassementCompleted += RecupererClassement;
            _serviceTaqnshareClient.RecupererClassementAsync();

            if (!App.EstAuthentifie && utilisateurConnecte)
                Loaded += ConnexionFacebookBoutonTap;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            FacebookConnexion();
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Permet de prendre une nouvelle photo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoutonPrendrePhotoTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _camera.Show();
        }

        /// <summary>
        /// Permet de sélectionner une photo dans la galerie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoutonSelectPhotoTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _galerie.Show();
        }

        /// <summary>
        /// Permet de récupérer la photo prise ou sélectionnée par l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Permet de gérer la liste des découpages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListPickerDecoupageChange(object sender, SelectionChangedEventArgs e)
        {
            SaveSettings("TailleGrille", ListPickerDecoupage, false);
            SaveSettings("IndexDecoupage", ListPickerDecoupage, true);
        }

        /// <summary>
        /// Permet de gérer la liste des filtres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListPickerFiltreChange(object sender, SelectionChangedEventArgs e)
        {
            SaveSettings("IndexFiltre", ListPickerFiltre, true);
        }

        /// <summary>
        /// Permet de faire persister les paramètres dans le téléphone
        /// </summary>
        /// <param name="key"></param>
        /// <param name="liste"></param>
        /// <param name="casStock"></param>
        private static void SaveSettings(String key, ListPicker liste, bool casStock)
        {
            Parametre p = (Parametre) liste.SelectedItem;
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

        /// <summary>
        /// Permet de lancer la connexion à Facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnexionFacebookBoutonTap(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AuthentificationFacebookPage.xaml?pageAvant=MainPage", UriKind.Relative));
        }

        /// <summary>
        /// Permet de gérer l'interface lors de la connexion/deconnexion de l'utilisateur
        /// </summary>
        private void FacebookConnexion()
        {
            if (App.EstAuthentifie)
            {
                ConnexionFacebookBouton.Visibility = Visibility.Collapsed;
                DeconnexionFacebookBouton.Visibility = Visibility.Visible;
                NomUtilisateurTextBlock.Visibility = Visibility.Visible;
                UtilisateurImage.Visibility = Visibility.Visible;
                RecupererInformationsUtilisateur();
            }
            else
            {
                ConnexionFacebookBouton.Visibility = Visibility.Visible;
                DeconnexionFacebookBouton.Visibility = Visibility.Collapsed;
                NomUtilisateurTextBlock.Visibility = Visibility.Collapsed;
                UtilisateurImage.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Permet de récupérer les infos de l'utilisateur actuel
        /// </summary>
        private void RecupererInformationsUtilisateur()
        {
            UtilisateurImage.Source = App.PhotoUtilisateur;
            Utilisateur utilisateurCourant = App.UtilisateurCourant;

            if (NomUtilisateurTextBlock != null)
                NomUtilisateurTextBlock.Text = String.Format("{0} {1}", utilisateurCourant.prenom_utilisateur, utilisateurCourant.nom_utilisateur);

            //Récupération des défis en attente de l'utilisateur
            _serviceTaqnshareClient.RecupererDefisEnAttenteCompleted += AfficherDefisEnAttente;
            _serviceTaqnshareClient.RecupererDefisEnAttenteAsync(App.UtilisateurCourant.id_utilisateur);

            //Récupération du rang de l'utilisateur
            _serviceTaqnshareClient.RecupererRangJoueurCompleted += RecupererRang;
            _serviceTaqnshareClient.RecupererRangJoueurAsync(App.UtilisateurCourant.id_utilisateur);

            //Récupération du score de l'utilisateur
            _serviceTaqnshareClient.RecupererScoreJoueurCompleted += RecupererScore;
            _serviceTaqnshareClient.RecupererScoreJoueurAsync(App.UtilisateurCourant.id_utilisateur);

        }

        /// <summary>
        /// Permet d'afficher les défis en attente de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfficherDefisEnAttente(object sender, RecupererDefisEnAttenteCompletedEventArgs e)
        {
            List<DefiService> listeDefiServices = e.Result;

            foreach (var defiService in listeDefiServices)
            {
                DefiAffiche defiAffiche = new DefiAffiche(defiService);
                _defisAAfficher.Add(defiAffiche);
            }

            AucunDefiEnAttenteTextBlock.Visibility = _defisAAfficher.Count <= 0 ? Visibility.Visible : Visibility.Collapsed;  
            
            DefisListBox.ItemsSource = _defisAAfficher;
        }

        /// <summary>
        /// Permet d'afficher les détails d'un défi avant de pouvoir l'accepter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfficherDetailDefiBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button bouton = (Button) sender;
            if (bouton.DataContext is DefiAffiche)
            {
                DefiAffiche defiAffiche = (DefiAffiche) bouton.DataContext;
                NavigationService.Navigate(new Uri("/Pages/AfficherDetailDefiPage.xaml?idDefi=" + defiAffiche.IdDefi,
                                                   UriKind.Relative));
            }

        }

        /// <summary>
        /// Permet à l'utilisateur de se déconnecter de Facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeconnexionFacebookBoutonTap(object sender, RoutedEventArgs e)
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

            AffichageRangScore();
        }

        /// <summary>
        /// Permet de récupérer le classement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecupererClassement(object sender, RecupererClassementCompletedEventArgs e)
        {
            List<UtilisateurService> utilisateurs = e.Result;
            int position = 1;

            foreach (UtilisateurService u in utilisateurs)
            {
                _classement.Add(new Classement { Position = position, Nom = u.NomUtilisateur, Prenom = u.PrenomUtilisateur, ScoreTotal = (float)(u.ScoreTotalUtilisateur / u.NombrePartieUtilisateur) });
                position++;
            }

            ClassementListBox.ItemsSource = _classement;
        }

        /// <summary>
        /// Affichage du score actuel de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecupererScore(object sender, RecupererScoreJoueurCompletedEventArgs e)
        {
            ScoreUtilisateurTextBlock.Text = "Score : " + e.Result;
        }

        /// <summary>
        /// Affichage du rang de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecupererRang(object sender, RecupererRangJoueurCompletedEventArgs e)
        {
            RangUtilisateurTextBlock.Text = "Rang : " + e.Result;
        }

        /// <summary>
        /// Gestion de l'interface
        /// </summary>
        private void AffichageRangScore()
        {
            if (App.EstAuthentifie)
            {
                RangUtilisateurTextBlock.Visibility = Visibility.Visible;
                ScoreUtilisateurTextBlock.Visibility = Visibility.Visible;
                defisUtilisateurs.Visibility = Visibility.Visible;
            }
            else
            {
                RangUtilisateurTextBlock.Visibility = Visibility.Collapsed;
                ScoreUtilisateurTextBlock.Visibility = Visibility.Collapsed;
                defisUtilisateurs.Visibility = Visibility.Collapsed;
                _defisAAfficher.Clear();
            }
        }

        private void DefisUtilisateursClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ListeDefisPage.xaml", UriKind.Relative));
        }    

        private void ClassementListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClassementListBox.SelectedItem = null;
        }

        /// <summary>
        /// Permet de bloquer le bouton retour du téléphone
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }
    }
}