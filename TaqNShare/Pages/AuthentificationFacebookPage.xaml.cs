using System;
using System.IO.IsolatedStorage;
using System.Windows;
using Facebook.Client;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace TaqNShare.Pages
{
    public partial class FacebookLoginPage
    {
        private string _pageAvant;

        public FacebookLoginPage()
        {
            InitializeComponent();

            Loaded += AuthentificationFacebookPageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); 
            string pa;
            if(NavigationContext.QueryString.TryGetValue("pageAvant",out pa))
            {
                _pageAvant = pa;
            }  
        }

        async void AuthentificationFacebookPageLoaded(object sender, RoutedEventArgs e)
        {
            if (!App.EstAuthentifie)
            {
                App.EstAuthentifie = true;
                await Authentification();
            }
        }

        private FacebookSession _session;
        private async Task Authentification()
        {
            App.FacebookSessionClient.Logout();
            try
            {
                _session = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream");
                App.AccessToken = _session.AccessToken;
                App.IdFacebook = _session.FacebookId;

                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                const string key = "UtilisateurConnecte";

                if (!settings.Contains(key))
                    settings.Add(key, true);
                else
                    settings[key] = true;

                settings.Save();
                
                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Pages/" + _pageAvant + ".xaml", UriKind.Relative)));
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show("Login failed! Exception details: " + e.Message);
            }
        }
    }
}