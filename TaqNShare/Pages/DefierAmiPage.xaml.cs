using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Facebook;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;


namespace TaqNShare.Pages
{
    public partial class DefierAmiPage
    {
        public ObservableCollection<InformationsAmis> ListeAmis { get; set; }
        private ObservableCollection<InformationsAmis> Amis = new ObservableCollection<InformationsAmis>();
        private Utilisateur u = App.UtilisateurCourant;

        public DefierAmiPage()
        {
            InitializeComponent();

            RecupererListeAmis();

            ListeAmis = Amis;

            DataContext = this;
        }

        private void RecupererListeAmis()
        {
            FacebookClient fb = new FacebookClient(App.AccessToken);
            
            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                var data = (IEnumerable<object>)result["data"];

                Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in data)
                    {
                        var ami = (IDictionary<string, object>)item;

                        Amis.Add(new InformationsAmis { Nom = (string)ami["name"], Id = (string)ami["id"], Image = (new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", ami["id"], "square", App.AccessToken)))});
                    }
                });
            };
            fb.GetTaskAsync("/me/friends");
        }

        private void retourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        private void defier_Click(object sender, RoutedEventArgs e)
        {
            if (listeAmis.SelectedIndex == -1)
        {
                MessageBox.Show("Vous devez sélectionner un ami.");
            }
            else if (nomDefi.Text == "")
            {
                MessageBox.Show("Le défi doit avoir un nom.");
            }
            else
                {
                MessageBox.Show("Numéro de l'ami dans la liste : " + listeAmis.SelectedIndex.ToString());
                MessageBox.Show("Id de l'ami : " + Amis[listeAmis.SelectedIndex].Id);
                MessageBox.Show(nomDefi.Text);
                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            }
                }

        public class InformationsAmis
                    {
            public string Id { get; set; }

            public string Nom { get; set; }

            public Uri Image { get; set; }
        }

        /// <summary>
        /// Méthode permettant la création d'un défi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefierAmiBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Partie partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];
            

            //Création du défi
            Defi defi = new Defi();
            defi.id_utilisateur = App.UtilisateurCourant.id_utilisateur;
            defi.nom_defi = "Test";
            defi.score_utilisateur_defi = partieTermine.Score;
            defi.resolu = false;
            defi.id_adversaire_defi = "Friend";

            //Récupération de la composition du taquin (Piece, position, filtre)
            List<Composer> listePiecePartie = new List<Composer>();
            foreach (var piece in partieTermine.ListePiecesInitale)
            {
                Composer composer = new Composer();
                composer.id_filtre = piece.IdFiltre;
                composer.id_piece = piece.Id;
                composer.position_piece = piece.IndexPosition;

                listePiecePartie.Add(composer);
            }

            //Gestion de la photo
            WriteableBitmap imageSelectionne = (WriteableBitmap)PhoneApplicationService.Current.State["photo"];
            byte[] imageAEnvoyer = Photo.ConvertToBytes(imageSelectionne);

            ServiceTaqnshareClient webServiceTaqnshareClient = new ServiceTaqnshareClient();
            webServiceTaqnshareClient.CreerDefiCompleted += Defier;
            webServiceTaqnshareClient.CreerDefiAsync(defi, listePiecePartie, imageAEnvoyer);

        }

        /// <summary>
        /// Méthode appeler par le web service lorsque la création du défi est finie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Defier(object sender, CreerDefiCompletedEventArgs e)
        {
            if (e.Result == "OK")
            {
                MessageBox.Show(e.Result);
                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
                PhoneApplicationService.Current.State.Clear();
            }
            else
                MessageBox.Show(e.Result);

        }
    }
}