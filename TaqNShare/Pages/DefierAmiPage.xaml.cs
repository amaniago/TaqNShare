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
        readonly Partie _partieTermine = (Partie)PhoneApplicationService.Current.State["partie"];
        readonly ServiceTaqnshareClient _webServiceTaqnshareClient = new ServiceTaqnshareClient();
        public ObservableCollection<UtilisateurFacebook> UtilisateurList { get; set; }

        public DefierAmiPage()
        {

            ListeAmis.Vider();
            //UtilisateurList.Clear();
            InitializeComponent();

            LoadUserInfo();
            RecupererListeAmis();

            UtilisateurList = ListeAmis.Amis;

            DataContext = this;
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
                });
            };

            fb.GetTaskAsync("me");
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
                    // The observable collection can only be updated from within the UI thread. See 
                    // http://10rem.net/blog/2012/01/10/threading-considerations-for-binding-and-change-notification-in-silverlight-5
                    // If you try to update the bound data structure from a different thread, you are going to get a cross
                    // thread exception.
                    foreach (var item in data)
                    {
                        var friend = (IDictionary<string, object>)item;

                        ListeAmis.Amis.Add(new UtilisateurFacebook { Nom = (string)friend["name"], Id = (string)friend["id"], Image = new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", friend["id"], "square", App.AccessToken)) });
                    }
                });

            };
            fb.GetTaskAsync("/me/friends");
        }

        private void DefierAmiBoutonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Defi defi = new Defi();
            defi.id_utilisateur = App.IdFacebook;
            defi.nom_defi = "Test";
            defi.score_utilisateur_defi = _partieTermine.Score;
            defi.resolu = false;
            defi.id_adversaire_defi = "Friend";

            List<Composer> listePiecePartie = new List<Composer>();
            foreach (var piece in _partieTermine.ListePiecesInitale)
            {
                Composer composer = new Composer();
                composer.id_filtre = piece.IdFiltre;
                composer.id_piece = piece.Id;
                composer.position_piece = piece.IndexPosition;

                listePiecePartie.Add(composer);
                
            }

            _webServiceTaqnshareClient.CreerDefiCompleted += Defier;
            _webServiceTaqnshareClient.CreerDefiAsync(defi, listePiecePartie);

        }

        private void Defier(object sender, CreerDefiCompletedEventArgs e)
        {
            if (e.Result == "OK")
            {
                MessageBox.Show(e.Result);
                NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            }
            else
                MessageBox.Show(e.Result);
            
        }
    }
}