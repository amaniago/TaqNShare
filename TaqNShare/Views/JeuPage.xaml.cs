using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using TaqNShare.Data;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace TaqNShare.Views
{
    /// <summary>
    /// Page de jeu :
    ///     -Préparation de l'image
    ///     -Déroulement du jeu
    /// </summary>
    public partial class JeuPage
    {
        public JeuPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PreparerImage();
            base.OnNavigatedTo(e);
        }

        private void PreparerImage()
        {
            int tailleGrille;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out tailleGrille);

            int hauteurPiece = 250;
            int largeurPiece = 150;

            switch (tailleGrille)
            {
                case 0:
                    tailleGrille = 3;
                    break;

                case 4:
                    hauteurPiece = 187;
                    largeurPiece = 112;
                    break;

                case 5:
                    hauteurPiece = 150;
                    largeurPiece = 90;
                    break;
            }

            //Récupération de la photo
            Photo photo = (Photo)PhoneApplicationService.Current.State["photo"];
            WriteableBitmap imageSelectionne = photo.PhotoSelectionne;

            //Découpage de la photo
            List<WriteableBitmap> listPiece = new List<WriteableBitmap>();
            for (int i = 0; i < tailleGrille; i++)
            {
                var cropped = imageSelectionne.Crop(i * largeurPiece, 0, largeurPiece, hauteurPiece);
                listPiece.Add(cropped);
                for (int j = 1; j < tailleGrille; j++)
                {
                    var cropped2 = imageSelectionne.Crop(i * largeurPiece, j * hauteurPiece, largeurPiece, hauteurPiece);
                    if (!(i == tailleGrille - 1 && j == tailleGrille - 1))
                        listPiece.Add(cropped2);
                }

            }

            //Création des colonnes de la grille
            for (int i = 0; i < tailleGrille; i++)
            {
                JeuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Create des lignes de la grille
            for (int i = 0; i < tailleGrille; i++)
            {
                JeuGrid.RowDefinitions.Add(new RowDefinition());
            }


            int compteur = 0;
            for (int i = 0; i < tailleGrille; i++)
            {
                for (int j = 0; j < tailleGrille; j++)
                {
                    if (!(i == tailleGrille - 1 && j == tailleGrille - 1))
                    {
                        Image image = new Image { Name = "image" + compteur, Source = listPiece[compteur] };
                        image.Tap += Deplacement;
                        Grid.SetRow(image, j);
                        Grid.SetColumn(image, i);
                        JeuGrid.Children.Add(image);
                        compteur++;
                    }
                }
            }

        }

        private void Deplacement(object sender, GestureEventArgs e)
        {
            /*
            Image i = sender as Image;

            int l = Grid.GetRow(i);
            int c = Grid.GetColumn(i);

            if (!(GetGridChildrenByCoord(JeuGrid, l, c+1)) && i != null)
            {
                JeuGrid.Children.Remove(i);
                Grid.SetRow(i, 2);
                Grid.SetColumn(i, 2);
                JeuGrid.Children.Add(i);
            }
            */
        }

        /// <summary>
        /// Méthode permettant de tester s'il y a un élement dans la grille à la position demandée
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool GetGridChildrenByCoord(int row, int column)
        {
            return JeuGrid.Children.Cast<FrameworkElement>().Any(element => (Grid.GetRow(element) == row) && (Grid.GetColumn(element) == column));
        }
    }
}