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
        public ObservableCollection<UtilisateurFacebook> ListeAmis { get; set; }
        private ObservableCollection<UtilisateurFacebook> Amis = new ObservableCollection<UtilisateurFacebook>();

        public DefierAmiPage()
        {
            InitializeComponent();

            LoadUserInfo();
            RecupererListeAmis();

            ListeAmis = Amis;

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
                    foreach (var item in data)
                    {
                        var ami = (IDictionary<string, object>)item;
                   
                        Amis.Add(new UtilisateurFacebook { Nom = (string)ami["name"], Id = (string)ami["id"], Image = new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", ami["id"], "square", App.AccessToken)) });
                    }
                });

            };
            fb.GetTaskAsync("/me/friends");
        }
    }
}