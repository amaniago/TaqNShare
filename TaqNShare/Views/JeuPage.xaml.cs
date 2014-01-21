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
        private int _tailleGrille;

        public JeuPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out _tailleGrille);
            PreparerImage();
            base.OnNavigatedTo(e);
        }

        private void PreparerImage()
        {
            int hauteurPiece = 250;
            int largeurPiece = 150;

            switch (_tailleGrille)
            {
                case 0:
                    _tailleGrille = 3;
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
            for (int i = 0; i < _tailleGrille; i++)
            {
                var cropped = imageSelectionne.Crop(i * largeurPiece, 0, largeurPiece, hauteurPiece);
                listPiece.Add(cropped);
                for (int j = 1; j < _tailleGrille; j++)
                {
                    var cropped2 = imageSelectionne.Crop(i * largeurPiece, j * hauteurPiece, largeurPiece, hauteurPiece);
                    if (!(i == _tailleGrille - 1 && j == _tailleGrille - 1))
                        listPiece.Add(cropped2);
                }

            }

            //Création des colonnes de la grille
            for (int i = 0; i < _tailleGrille; i++)
            {
                JeuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Create des lignes de la grille
            for (int i = 0; i < _tailleGrille; i++)
            {
                JeuGrid.RowDefinitions.Add(new RowDefinition());
            }


            int compteur = 0;
            for (int i = 0; i < _tailleGrille; i++)
            {
                for (int j = 0; j < _tailleGrille; j++)
                {
                    if (!(i == _tailleGrille - 1 && j == _tailleGrille - 1))
                    {
                        Image image = new Image { Name = "image" + compteur, Source = listPiece[compteur] };
                        image.Tap += ImageTap;
                        Grid.SetRow(image, j);
                        Grid.SetColumn(image, i);
                        JeuGrid.Children.Add(image);
                        compteur++;
                    }
                }
            }

        }


        struct PositionElement
        {
            public int Ligne { get; private set; }
            public int Colonne { get; private set; }

            public PositionElement(FrameworkElement imageCliquee)
                : this()
            {
                Ligne = Grid.GetRow(imageCliquee);
                Colonne = Grid.GetColumn(imageCliquee);
            }
        }

        /// <summary>
        /// Méthode permettant l'appel de la fonction de déplacement suivant 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageTap(object sender, GestureEventArgs e)
        {
            Image imageCliquee = sender as Image;
            PositionElement positionImageCliquee = new PositionElement(imageCliquee);

            //Si l'image est sur le bord en haut de la grille
            if (EstSurBordHaut(positionImageCliquee))
                //Déplacement haut, gauche ou droit possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, false, true, true, true);

            //Si l'image est sur le bord en bas de la grille
            else if (EstSurBordBas(positionImageCliquee))
                //Déplacement bas, gauche ou droit possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, true, false, true, true);

            //Si l'image est sur le bord à gauche de la grille
            else if (EstSurBordGauche(positionImageCliquee))
                //Déplacement haut, bas ou droit possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, true, true, false, true);

            //Si l'image est sur le bord à droite de la grille
            else if (EstSurBordDroit(positionImageCliquee))
                //Déplacement haut, bas ou gauche possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, true, true, true, false);

            //Si l'image est dans le coin en haut à gauche de la grille
            else if (EstSurCoinHautGauche(positionImageCliquee))
                //Déplacement bas ou droite possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, false, true, false, true);

            //Si l'image est dans le coin en haut à droite de la grille
            else if (EstSurCoinHautDroit(positionImageCliquee))
                //Déplacement bas ou gauche possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, false, true, true, false);

            //Si l'image est dans le coin en bas à gauche de la grille
            else if (EstSurCoinBasGauche(positionImageCliquee))
                //Déplacement haut ou droite possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, true, false, false, true);

            //Si l'image est dans le coin en bas à droite de la grille
            else if (EstSurCoinBasDroit(positionImageCliquee))
                //Déplacement haut ou gauche possible
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, true, false, true, false);

            //Si l'image au milieu dans la grille
            else
                //Tous les déplacements possibles
                DeplacementSuivantPosition(imageCliquee, positionImageCliquee, true, true, true, true);

        }

        /// <summary>
        /// Méthode permettant de déplacer l'image
        /// </summary>
        /// <param name="imageCliquee"></param>
        /// <param name="positionImageCliquee"></param>
        /// <param name="haut"></param>
        /// <param name="bas"></param>
        /// <param name="gauche"></param>
        /// <param name="droite"></param>
        private void DeplacementSuivantPosition(FrameworkElement imageCliquee, PositionElement positionImageCliquee, bool haut, bool bas, bool gauche, bool droite)
        {
            if (haut)
                if (!(ElementPresent(positionImageCliquee.Ligne - 1, positionImageCliquee.Colonne)))
                    Grid.SetRow(imageCliquee, positionImageCliquee.Ligne - 1);

            if (bas)
                if (!(ElementPresent(positionImageCliquee.Ligne + 1, positionImageCliquee.Colonne)))
                    Grid.SetRow(imageCliquee, positionImageCliquee.Ligne + 1);

            if (gauche)
                if (!(ElementPresent(positionImageCliquee.Ligne, positionImageCliquee.Colonne - 1)))
                    Grid.SetColumn(imageCliquee, positionImageCliquee.Colonne - 1);

            if (droite)
                if (!(ElementPresent(positionImageCliquee.Ligne, positionImageCliquee.Colonne + 1)))
                    Grid.SetColumn(imageCliquee, positionImageCliquee.Colonne + 1);
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord haut de la grille
        /// </summary>
        /// <param name="positionImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordHaut(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Ligne == 0) && (positionImageCliquee.Colonne != 0) && (positionImageCliquee.Colonne != _tailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord bas de la grille
        /// </summary>
        /// <param name="positionImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordBas(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Ligne == _tailleGrille - 1) && (positionImageCliquee.Colonne != 0) && (positionImageCliquee.Colonne != _tailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord gauche de la grille
        /// </summary>
        /// <param name="positionImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordGauche(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Colonne == 0) && (positionImageCliquee.Ligne != 0) && (positionImageCliquee.Ligne != _tailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord droit de la grille
        /// </summary>
        /// <param name="positionImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordDroit(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Colonne == _tailleGrille - 1) && (positionImageCliquee.Ligne != 0) && (positionImageCliquee.Ligne != _tailleGrille - 1);
        }

        private bool EstSurCoinHautGauche(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Colonne == 0) && (positionImageCliquee.Ligne == 0);
        }

        private bool EstSurCoinHautDroit(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Colonne == _tailleGrille - 1) && (positionImageCliquee.Ligne == 0);
        }

        private bool EstSurCoinBasGauche(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Colonne == 0) && (positionImageCliquee.Ligne == _tailleGrille - 1);
        }

        private bool EstSurCoinBasDroit(PositionElement positionImageCliquee)
        {
            return (positionImageCliquee.Colonne == _tailleGrille - 1) && (positionImageCliquee.Ligne == _tailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de tester s'il y a un élement dans la grille à la position demandée
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool ElementPresent(int row, int column)
        {
            return JeuGrid.Children.Cast<FrameworkElement>().Any(element => (Grid.GetRow(element) == row) && (Grid.GetColumn(element) == column));
        }
    }
}