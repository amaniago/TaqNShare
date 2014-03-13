using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using Facebook;
using Microsoft.Phone.Shell;
using TaqNShare.Donnees;
using TaqNShare.TaqnshareReference;


namespace TaqNShare.Pages
{
    public partial class DefierAmiPage
    {
        //Ces deux variables sont des listes de type AmisFacebook qui permettent de stocker les informations des amis et le binding
        public ObservableCollection<AmisFacebook> ListeAmis { get; set; }//Pour le binding
        private readonly ObservableCollection<AmisFacebook> _amis = new ObservableCollection<AmisFacebook>();

        public DefierAmiPage()
        {
            InitializeComponent();

            RecupererListeAmis();

            ListeAmis = _amis;

            DataContext = this;
        }

        private void RecupererListeAmis()
        {
            //On se connecte à la session Facebook de l'utilisateur à partir du token
            FacebookClient fb = new FacebookClient(App.AccessToken);

            //On récupère la liste des amis en parcourant le graphe Facebook.
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

                                                                  _amis.Add(new AmisFacebook
                                                                            {
                                                                                Nom =
                                                                                    (string)
                                                                                    ami["name"],
                                                                                Id =
                                                                                    (string)
                                                                                    ami["id"],
                                                                                Image =
                                                                                    (new Uri(
                                                                                    string.Format(
                                                                                        "https://graph.facebook.com/{0}/picture?type={1}&access_token={2}",
                                                                                        ami["id"],
                                                                                        "square",
                                                                                        App
                                                                                    .AccessToken)))
                                                                            });
                                                              }
                                                          });
                               };
            fb.GetTaskAsync("/me/friends");
        }

        /// <summary>
        /// Méthode permettant la création d'un défi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefierAmiBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Partie partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];

            if (AmisListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Vous devez sélectionner un ami !");
            }
            else if (NomDefiTextBox.Text == "")
            {
                MessageBox.Show("Vous devez donner un nom à votre défi !");
            }
            else
            {
                DefierAmiBouton.IsEnabled = false;
                //Création du défi
                Defi defi = new Defi();
                defi.id_utilisateur = App.UtilisateurCourant.id_utilisateur;
                defi.nom_defi = NomDefiTextBox.Text;
                defi.score_utilisateur_defi = partieTermine.Score;
                defi.resolu = false;
                defi.id_adversaire_defi = _amis[AmisListBox.SelectedIndex].Id;

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
                byte[] imageAEnvoyer = Photo.ConvertToBytes(partieTermine.Photo);

                ServiceTaqnshareClient webServiceTaqnshareClient = new ServiceTaqnshareClient();
                webServiceTaqnshareClient.CreerDefiCompleted += Defier;
                webServiceTaqnshareClient.CreerDefiAsync(defi, listePiecePartie, imageAEnvoyer);

            }
        }

        /// <summary>
        /// Méthode appeler par le web service lorsque la création du défi est finie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Defier(object sender, CreerDefiCompletedEventArgs e)
        {
            if (e.Result)
            {
                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
                PhoneApplicationService.Current.State.Clear();
            }
            else
                MessageBox.Show(e.Result.ToString());
        }

        //Permet de bloquer le bouton retour du téléphone
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }
    }
}