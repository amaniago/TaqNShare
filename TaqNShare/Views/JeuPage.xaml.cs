using System;
using System.Globalization;
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
        private Partie _partieEnCours;

        public JeuPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int tailleGrille;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out tailleGrille);
            //Création de la partie
            _partieEnCours = new Partie(tailleGrille);
            //Spécification du dataContext pour le Binding
            DataContext = _partieEnCours;

            PreparerImage();
            base.OnNavigatedTo(e);
        }
   
        

        private void PreparerImage()
        {
            int hauteurPiece = 244;
            int largeurPiece = 150;

            switch (_partieEnCours.TailleGrille)
            {
                case 0:
                    _partieEnCours.TailleGrille = 3;
                    break;

                case 4:
                    hauteurPiece = 183;
                    largeurPiece = 112;
                    break;

                case 5:
                    hauteurPiece = 146;
                    largeurPiece = 90;
                    break;
            }

            //Récupération de la photo
            Photo photo = (Photo)PhoneApplicationService.Current.State["photo"];
            WriteableBitmap imageSelectionne = photo.PhotoSelectionne;


            //Création des colonnes de la grille
            for (int i = 0; i < _partieEnCours.TailleGrille; i++)
            {
                JeuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Create des lignes de la grille
            for (int i = 0; i < _partieEnCours.TailleGrille; i++)
            {
                JeuGrid.RowDefinitions.Add(new RowDefinition());
            }

            //Découpage de la photo  
            int compteur = 0;
            for (int i = 0; i < _partieEnCours.TailleGrille; i++)
            {

                for (int j = 0; j < _partieEnCours.TailleGrille; j++)
                {
                    if (!(i == _partieEnCours.TailleGrille - 1 && j == _partieEnCours.TailleGrille - 1))
                    {
                        Image image = new Image
                        {
                            Name = compteur.ToString(CultureInfo.InvariantCulture),
                            Source = imageSelectionne.Crop(i * largeurPiece, j * hauteurPiece, largeurPiece, hauteurPiece)
                        };
                        image.Tap += ImageTap;
                        Grid.SetRow(image, j);
                        Grid.SetColumn(image, i);
                        JeuGrid.Children.Add(image);
                        compteur++;
                        _partieEnCours.ListePieces.Add(image);
                    }
                }
            }

            Melange();

        }

        private void Melange()
        {
            Random random = new Random();

            //Début boucle
            for (int i = 0; i < 1000; i++)
            {
                int indexRandom = random.Next(_partieEnCours.ListePieces.Count);

                //Prise d'une pièce au hasard dans la liste
                var piece = _partieEnCours.ListePieces[indexRandom];

                CoordonneeElement coordonneeImageCliquee = new CoordonneeElement(piece);

                //Si l'image est sur le bord en haut de la grille
                if (EstSurBordHaut(coordonneeImageCliquee))
                    //Déplacement haut, gauche ou droit possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, false, true, true, true);

                    //Si l'image est sur le bord en bas de la grille
                else if (EstSurBordBas(coordonneeImageCliquee))
                    //Déplacement bas, gauche ou droit possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, true, false, true, true);

                    //Si l'image est sur le bord à gauche de la grille
                else if (EstSurBordGauche(coordonneeImageCliquee))
                    //Déplacement haut, bas ou droit possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, true, true, false, true);

                    //Si l'image est sur le bord à droite de la grille
                else if (EstSurBordDroit(coordonneeImageCliquee))
                    //Déplacement haut, bas ou gauche possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, true, true, true, false);

                    //Si l'image est dans le coin en haut à gauche de la grille
                else if (EstSurCoinHautGauche(coordonneeImageCliquee))
                    //Déplacement bas ou droite possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, false, true, false, true);

                    //Si l'image est dans le coin en haut à droite de la grille
                else if (EstSurCoinHautDroit(coordonneeImageCliquee))
                    //Déplacement bas ou gauche possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, false, true, true, false);

                    //Si l'image est dans le coin en bas à gauche de la grille
                else if (EstSurCoinBasGauche(coordonneeImageCliquee))
                    //Déplacement haut ou droite possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, true, false, false, true);

                    //Si l'image est dans le coin en bas à droite de la grille
                else if (EstSurCoinBasDroit(coordonneeImageCliquee))
                    //Déplacement haut ou gauche possible
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, true, false, true, false);

                    //Si l'image au milieu dans la grille
                else
                    //Tous les déplacements possibles
                    DeplacementSuivantPosition(piece, coordonneeImageCliquee, true, true, true, true);

            }

            //Déterminer si elle est déplacable
            //Stocker toutes les pièces déplacables
            //Random sur la liste.
            //Déplacement de la pièce choisie
            //Fin boucle
        }

        //TODO : Faire fonction de détermination de la position
        private string DeterminerPositionElement(CoordonneeElement coordonneeImageCliquee)
        {
            string position = "";


            return position;
        }

        private struct CoordonneeElement
        {
            public int Ligne { get; private set; }
            public int Colonne { get; private set; }

            public CoordonneeElement(FrameworkElement imageCliquee)
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
            CoordonneeElement coordonneeImageCliquee = new CoordonneeElement(imageCliquee);
            string positionElementGrille = DeterminerPositionElement(coordonneeImageCliquee);
            //Si l'image est sur le bord en haut de la grille
            if (EstSurBordHaut(coordonneeImageCliquee))
                //Déplacement haut, gauche ou droit possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, false, true, true, true);

                //Si l'image est sur le bord en bas de la grille
            else if (EstSurBordBas(coordonneeImageCliquee))
                //Déplacement bas, gauche ou droit possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, true, false, true, true);

                //Si l'image est sur le bord à gauche de la grille
            else if (EstSurBordGauche(coordonneeImageCliquee))
                //Déplacement haut, bas ou droit possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, true, true, false, true);

                //Si l'image est sur le bord à droite de la grille
            else if (EstSurBordDroit(coordonneeImageCliquee))
                //Déplacement haut, bas ou gauche possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, true, true, true, false);

                //Si l'image est dans le coin en haut à gauche de la grille
            else if (EstSurCoinHautGauche(coordonneeImageCliquee))
                //Déplacement bas ou droite possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, false, true, false, true);

                //Si l'image est dans le coin en haut à droite de la grille
            else if (EstSurCoinHautDroit(coordonneeImageCliquee))
                //Déplacement bas ou gauche possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, false, true, true, false);

                //Si l'image est dans le coin en bas à gauche de la grille
            else if (EstSurCoinBasGauche(coordonneeImageCliquee))
                //Déplacement haut ou droite possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, true, false, false, true);

                //Si l'image est dans le coin en bas à droite de la grille
            else if (EstSurCoinBasDroit(coordonneeImageCliquee))
                //Déplacement haut ou gauche possible
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, true, false, true, false);

                //Si l'image au milieu dans la grille
            else
                //Tous les déplacements possibles
                DeplacementSuivantPosition(imageCliquee, coordonneeImageCliquee, true, true, true, true);

        }

        /// <summary>
        /// Méthode permettant de déplacer l'image
        /// </summary>
        /// <param name="imageCliquee"></param>
        /// <param name="coordonneeImageCliquee"></param>
        /// <param name="haut"></param>
        /// <param name="bas"></param>
        /// <param name="gauche"></param>
        /// <param name="droite"></param>
        private void DeplacementSuivantPosition(FrameworkElement imageCliquee, CoordonneeElement coordonneeImageCliquee,
            bool haut, bool bas, bool gauche, bool droite)
        {
            if (haut)
                if (!(ElementPresent(coordonneeImageCliquee.Ligne - 1, coordonneeImageCliquee.Colonne)))
                {
                    Grid.SetRow(imageCliquee, coordonneeImageCliquee.Ligne - 1);
                    _partieEnCours.NombreDeplacement++;
                }

            if (bas)
                if (!(ElementPresent(coordonneeImageCliquee.Ligne + 1, coordonneeImageCliquee.Colonne)))
                {
                    Grid.SetRow(imageCliquee, coordonneeImageCliquee.Ligne + 1);
                    _partieEnCours.NombreDeplacement++;
                }

            if (gauche)
                if (!(ElementPresent(coordonneeImageCliquee.Ligne, coordonneeImageCliquee.Colonne - 1)))
                {
                    Grid.SetColumn(imageCliquee, coordonneeImageCliquee.Colonne - 1);
                    _partieEnCours.NombreDeplacement++;
                }

            if (droite)
                if (!(ElementPresent(coordonneeImageCliquee.Ligne, coordonneeImageCliquee.Colonne + 1)))
                {
                    Grid.SetColumn(imageCliquee, coordonneeImageCliquee.Colonne + 1);
                    _partieEnCours.NombreDeplacement++;
                }
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord haut de la grille
        /// </summary>
        /// <param name="coordonneeImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordHaut(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Ligne == 0) && (coordonneeImageCliquee.Colonne != 0) &&
                   (coordonneeImageCliquee.Colonne != _partieEnCours.TailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord bas de la grille
        /// </summary>
        /// <param name="coordonneeImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordBas(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Ligne == _partieEnCours.TailleGrille - 1) &&
                   (coordonneeImageCliquee.Colonne != 0) &&
                   (coordonneeImageCliquee.Colonne != _partieEnCours.TailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord gauche de la grille
        /// </summary>
        /// <param name="coordonneeImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordGauche(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == 0) && (coordonneeImageCliquee.Ligne != 0) &&
                   (coordonneeImageCliquee.Ligne != _partieEnCours.TailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de dire si l'image cliquée est sur le bord droit de la grille
        /// </summary>
        /// <param name="coordonneeImageCliquee"></param>
        /// <returns></returns>
        private bool EstSurBordDroit(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == _partieEnCours.TailleGrille - 1) &&
                   (coordonneeImageCliquee.Ligne != 0) && (coordonneeImageCliquee.Ligne != _partieEnCours.TailleGrille - 1);
        }

        private bool EstSurCoinHautGauche(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == 0) && (coordonneeImageCliquee.Ligne == 0);
        }

        private bool EstSurCoinHautDroit(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == _partieEnCours.TailleGrille - 1) &&
                   (coordonneeImageCliquee.Ligne == 0);
        }

        private bool EstSurCoinBasGauche(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == 0) &&
                   (coordonneeImageCliquee.Ligne == _partieEnCours.TailleGrille - 1);
        }

        private bool EstSurCoinBasDroit(CoordonneeElement coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == _partieEnCours.TailleGrille - 1) &&
                   (coordonneeImageCliquee.Ligne == _partieEnCours.TailleGrille - 1);
        }

        /// <summary>
        /// Méthode permettant de tester s'il y a un élement dans la grille à la position demandée
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool ElementPresent(int row, int column)
        {
            return
                JeuGrid.Children.Cast<FrameworkElement>()
                    .Any(element => (Grid.GetRow(element) == row) && (Grid.GetColumn(element) == column));
        }



    }
}