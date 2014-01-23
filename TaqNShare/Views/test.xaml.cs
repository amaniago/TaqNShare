using System;
using System.Collections.Generic;
using System.Windows;
using Facebook;
using System.Windows.Media.Imaging;
using TaqNShare.Data;

namespace TaqNShare.Views
{
    public partial class Test
    {
        public Test()
        {
            InitializeComponent();
            LoadUserInfo();
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
                    var profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", App.FacebookId, "square", App.AccessToken);

                    MyImage.Source = new BitmapImage(new Uri(profilePictureUrl));
                    MyName.Text = String.Format("{0} {1}", result["first_name"], result["last_name"]);
                });
            };

            fb.GetTaskAsync("me");
        }

        private void friendSelectorTextBlockHandler(object sender, System.Windows.Input.GestureEventArgs evtArgs)
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

                        ListeAmis.Friends.Add(new UtilisateurFacebook { Nom = (string)friend["name"], Id = (string)friend["id"], Image = new Uri(string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", friend["id"], "square", App.AccessToken)) });
                    }

                    NavigationService.Navigate(new Uri("/Views/FriendSelector.xaml", UriKind.Relative));
                });

            };

            fb.GetTaskAsync("/me/friends");
        }
    }
}