using System;
using System.IO.IsolatedStorage;
using System.Windows;
using Facebook.Client;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;

namespace TaqNShare.Views
{
    public partial class FacebookLoginPage
    {
        public FacebookLoginPage()
        {
            InitializeComponent();

            Loaded += FacebookLoginPageLoaded;

        }

        async void FacebookLoginPageLoaded(object sender, RoutedEventArgs e)
        {
            if (!App.isAuthenticated)
            {
                App.isAuthenticated = true;
                await Authenticate();
            }
        }

        private FacebookSession _session;
        private async Task Authenticate()
        {
            App.FacebookSessionClient.Logout();
            try
            {
                _session = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream");
                App.AccessToken = _session.AccessToken;
                App.FacebookId = _session.FacebookId;



                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                string key = "UtilisateurConnecte";

                if (!settings.Contains(key))
                    settings.Add(key, true);
                else
                    settings[key] = true;


                settings.Save();


                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative)));
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show("Login failed! Exception details: " + e.Message);
            }
        }
    }
}