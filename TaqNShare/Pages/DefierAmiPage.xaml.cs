using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using Facebook;
using TaqNShare.Donnees;

namespace TaqNShare.Pages
{
    public partial class DefierAmiPage
    {
        public ObservableCollection<UtilisateurFacebook> UtilisateurList { get; set; }

        List<UtilisateurFacebook> amis = new List<UtilisateurFacebook>();

        public DefierAmiPage()
        {

            ListeAmis.Vider();
            //UtilisateurList.Clear();
            InitializeComponent();

            LoadUserInfo();
            RecupererListeAmis();

            UtilisateurList = amis;

            DataContext = this;
        }

        private void retourAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
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
                   
                        amis.Ajouter(new UtilisateurFacebook { Nom = (string)friend["name"], Id = (string)friend["id"], Image = new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", friend["id"], "square", App.AccessToken)) });
                    }
                });

            };
            fb.GetTaskAsync("/me/friends");
        }
    }
}