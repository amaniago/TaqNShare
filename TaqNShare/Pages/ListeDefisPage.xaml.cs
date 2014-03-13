using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using TaqNShare.TaqnshareReference;
using TaqNShare.Donnees;

namespace TaqNShare.Pages
{
    /// <summary>
    /// Classe permettant de lister les défis terminés par l'utilisateur
    /// </summary>
    public partial class ListeDefisPage : PhoneApplicationPage
    {
        private ServiceTaqnshareClient service = new ServiceTaqnshareClient();//WebService permettant de récupérer le classement dans la base de données
        public ObservableCollection<ListeDefis> Defis { get; set; }//Liste des défis pour le binding
        private ObservableCollection<ListeDefis> DefisListe = new ObservableCollection<ListeDefis>();//Liste pour récupérer les défis

        public ListeDefisPage()
        {
            InitializeComponent();
            RecupererListeDefis();
            Defis = DefisListe;

            DataContext = this;
        }

        public void RecupererListeDefis()
        {
            //Appel de la méthode du Webservice permettant de récupérer la liste des amis.
            service.RecupererDefisUtilisateurCompleted += RecupererDefisUtilisateur;
            service.RecupererDefisUtilisateurAsync(App.UtilisateurCourant.id_utilisateur);
        }

        private void RecupererDefisUtilisateur(object sender, RecupererDefisUtilisateurCompletedEventArgs e)
        {
            List<DefiService> defis = e.Result;

            //On ajoute un après l'autre à la liste les défis récupérés dans la base à l'aide du webservice
            foreach (DefiService d in defis)
            {
                DefisListe.Add(new ListeDefis {Nom = d.NomDefi, Utilisateur = d.PrenomUtilisateur + " " + d.NomUtilisateur + " : " + d.ScoreUtilisateurDefi.ToString(), Adversaire = d.PrenomAdversaire + " " + d.NomAdversaire + " : " + d.ScoreAdversaireDefi.ToString()});
            }
        }

        /// <summary>
        /// Méthode permettant le retour à l'accueil 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccueilTap(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        //Permet de bloquer le bouton retour du téléphone
        private void DefisList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            defisList.SelectedItem = null;
        }
    }
}