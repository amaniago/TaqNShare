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
        private string _pageAvant; //Contient le nom de la page sur laquelle on va rediriger après que l'authentification ait réussi.
        private string _idFacebook; //Contient l'id Facebook de l'utilisateur.
        private FacebookSession _session; //Permet d'ouvrir une session Facebook pour se connecter.

        public FacebookLoginPage()
        {
            InitializeComponent();

            //Une fois que la page est chargée on appelle la méthode d'authentification Facebook.
            Loaded += AuthentificationFacebookPageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); 
            string pa;
            //_pageAvant reçoit le nom de la page sur laquelle on va rediriger après que l'authentification ait réussi.
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
       
        private async Task Authentification()
        {
            //La variable FacebookSessionClient affiche automatique un élément de type WebBrowser pour la connection Facebook.
            //On déconnecte la variable de type FacebookSession.
            App.FacebookSessionClient.Logout();
            try
            {
                //On initialise la variable FaceBookSession et on récupère le token généré ainsi que l'id Facebook.
                _session = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream");
                App.AccessToken = _session.AccessToken;
                _idFacebook = _session.FacebookId;

                //Permet de savoir si l'utilisateur est déjà connecté à Facebook ou non.
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                const string key = "UtilisateurConnecte";

                if (!settings.Contains(key))
                    settings.Add(key, true);
                else
                    settings[key] = true;

                settings.Save();

                //Permet d'exploiter le graphe pour récupérer les informations concernant l'utilisateur.
                //On utilise un Task pour controler l'asynchronisme et permettre à l'application d'avoir le temps de récupérer les informations avant le changement de page.
                var fb = new FacebookClient(App.AccessToken);
                JsonObject data = await GetAsyncEx(fb,"https://graph.facebook.com/me", null);
                Dispatcher.BeginInvoke(() =>
                {
                    //On récupère l'image de l'utilisateur en parcourant le graphe Facebook.
                    var profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", _idFacebook, "square", App.AccessToken);
                    BitmapImage photo = new BitmapImage(new Uri(profilePictureUrl));
                    App.PhotoUtilisateur = photo;

                    //On crée un utilisateur qui récupère les informations de l'utilisateur.
                    Utilisateur u = new Utilisateur();
                    u.id_utilisateur = _idFacebook;
                    u.prenom_utilisateur = data["first_name"].ToString();
                    u.nom_utilisateur = data["last_name"].ToString();
                    //La variable globale de l'application récupère les informations de l'utilisateur.
                    App.UtilisateurCourant = u;
                });
                //On redirige vers la page dont le nom est contenu dans la variable _pageAvant.
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


        