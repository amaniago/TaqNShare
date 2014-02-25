using System;
using System.IO.IsolatedStorage;
using System.Windows;
using Facebook.Client;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Facebook;
using System.Windows.Media.Imaging;
using TaqNShare.TaqnshareReference;

namespace TaqNShare.Pages
{
    public partial class FacebookLoginPage
    {
        private string _pageAvant;
        private string _idFacebook;

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
                _idFacebook = _session.FacebookId;

                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                const string key = "UtilisateurConnecte";

                if (!settings.Contains(key))
                    settings.Add(key, true);
                else
                    settings[key] = true;

                settings.Save();

                var fb = new FacebookClient(App.AccessToken);
                JsonObject data = await GetAsyncEx(fb,"https://graph.facebook.com/me", null);
                Dispatcher.BeginInvoke(() =>
                {
                    var profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", _idFacebook, "square", App.AccessToken);
                    BitmapImage photo = new BitmapImage(new Uri(profilePictureUrl));
                    App.PhotoUtilisateur = photo;

                    Utilisateur u = new Utilisateur();
                    u.id_utilisateur = _idFacebook;
                    u.prenom_utilisateur = data["first_name"].ToString();
                    u.nom_utilisateur = data["last_name"].ToString();
                    App.UtilisateurCourant = u;
                });

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Pages/" + _pageAvant + ".xaml", UriKind.Relative)));
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show("Login failed! Exception details: " + e.Message);
            }
        }

        public static Task<JsonObject> GetAsyncEx(FacebookClient facebookClient, string uri, object parameters)
        {
            TaskCompletionSource<JsonObject> taskCompletionSource = new TaskCompletionSource<JsonObject>();
            EventHandler<FacebookApiEventArgs> getCompletedHandler = null;
            getCompletedHandler = (s, e) =>
            {
                facebookClient.GetCompleted -= getCompletedHandler;
                if (e.Error != null)
                    taskCompletionSource.TrySetException(e.Error);
                else
                    taskCompletionSource.TrySetResult((JsonObject)e.GetResultData());
            };

            facebookClient.GetCompleted += getCompletedHandler;
            facebookClient.GetAsync(uri, parameters);

            return taskCompletionSource.Task;
        }
    }
}


        