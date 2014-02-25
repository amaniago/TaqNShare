using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using Facebook;
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
    }
}