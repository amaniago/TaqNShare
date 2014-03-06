using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class ListeDefisPage : PhoneApplicationPage
    {
        private ServiceTaqnshareClient service = new ServiceTaqnshareClient();
        public ObservableCollection<ListeDefis> Defis { get; set; }
        private ObservableCollection<ListeDefis> DefisListe = new ObservableCollection<ListeDefis>();

        public ListeDefisPage()
        {
            InitializeComponent();
            RecupererListeDefis();
            Defis = DefisListe;

            DataContext = this;
        }

        public void RecupererListeDefis()
        {
            service.RecupererDefisUtilisateurCompleted += RecupererDefisUtilisateur;
            service.RecupererDefisUtilisateurAsync(App.UtilisateurCourant.id_utilisateur);
        }

        private void RecupererDefisUtilisateur(object sender, RecupererDefisUtilisateurCompletedEventArgs e)
        {
            List<DefiService> defis = e.Result;

            foreach (DefiService d in defis)
            {
                DefisListe.Add(new ListeDefis {Nom = d.NomDefi, Utilisateur = d.PrenomUtilisateur + " " + d.NomUtilisateur + " : " + d.ScoreUtilisateurDefi.ToString(), Adversaire = d.PrenomAdversaire + " " + d.NomAdversaire + " : " + d.ScoreAdversaireDefi.ToString()});
            }
        }

        private void AccueilClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }
    }

    public class ListeDefis
    {
        public string Nom { get; set; }

        public string Utilisateur { get; set; }

        public string Adversaire { get; set; }
    }
}