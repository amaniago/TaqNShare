using System;
using System.Windows;
using Facebook.Client;
using System.Threading.Tasks;

namespace TaqNShare.Views
{
    public partial class FacebookLoginPage
    {
        public FacebookLoginPage()
        {
            InitializeComponent();
            Loaded += FacebookLoginPage_Loaded;
        }

        async void FacebookLoginPage_Loaded(object sender, RoutedEventArgs e)
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
            try
            {
                _session = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream");
                App.AccessToken = _session.AccessToken;
                App.FacebookId = _session.FacebookId;

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/test.xaml", UriKind.Relative)));
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show("Login failed! Exception details: " + e.Message);
            }
        }
    }
}