using System;
using System.Collections.Generic;
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
                        Piece piece = new Piece(image);
                        _partieEnCours.ListePieces.Add(piece);
                    }
                }
            }

            Melange();

        }

        private void Melange()
        {
            Random random = new Random();

            //Début boucle pour effectuer 500 déplacements
            for (int i = 0; i < 500; i++)
            {
                //Récupération des pièces déplacables
                List<Piece> listePiecesDeplacables = _partieEnCours.ListePieces.Where(EstDeplacable).ToList();

                //Prise d'une pièce au hasard dans la liste des pièces déplacables
                int indexRandom = random.Next(listePiecesDeplacables.Count);
                var pieceADeplacer = listePiecesDeplacables[indexRandom];

                int tailleListe = _partieEnCours.ListePieces.Count;
                for (int index = 0; index < tailleListe; index++)
                {
                    var piece = _partieEnCours.ListePieces[index];
                    if (piece.Equals(pieceADeplacer))
                    {
                        _partieEnCours.ListePieces.Remove(piece);
                        tailleListe--;
                    }
                }

                DeplacementSuivantPosition(pieceADeplacer, true);

                pieceADeplacer.Ajuster();

                _partieEnCours.ListePieces.Add(pieceADeplacer);

            }//Fin boucle           
        }
        
        /// <summary>
        /// Méthode permettant l'appel de la fonction de déplacement suivant 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageTap(object sender, GestureEventArgs e)
        {
            int tailleListe = _partieEnCours.ListePieces.Count;
            Piece piececlique = new Piece();

            for (int index = 0; index < tailleListe; index++)
            {
                var piece = _partieEnCours.ListePieces[index];
                if (piece.Image.Equals(sender as Image))
                {
                    piececlique = piece;
                    tailleListe--;
                }
            }

            DeplacementSuivantPosition(piececlique, false);

            piececlique.Ajuster();

            _partieEnCours.ListePieces.Add(piececlique);

        }

        private bool EstDeplacable(Piece piece)
        {
            if (piece.DeplacementHaut)
                if (!(PiecePresente(piece.Coordonnee.Ligne - 1, piece.Coordonnee.Colonne)))
                    return true;

            if (piece.DeplacementBas)
                if (!(PiecePresente(piece.Coordonnee.Ligne + 1, piece.Coordonnee.Colonne)))
                    return true;

            if (piece.DeplacementGauche)
                if (!(PiecePresente(piece.Coordonnee.Ligne, piece.Coordonnee.Colonne - 1)))
                    return true;

            if (piece.DeplacementDroite)
                if (!(PiecePresente(piece.Coordonnee.Ligne, piece.Coordonnee.Colonne + 1)))
                    return true;

            return false;
        }

        /// <summary>
        /// Méthode permettant de déplacer l'image
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="casMelange"></param>
        private void DeplacementSuivantPosition(Piece piece, bool casMelange)
        {
            if (piece.DeplacementHaut)
                if (!(PiecePresente(piece.Coordonnee.Ligne - 1, piece.Coordonnee.Colonne)))
                {
                    Grid.SetRow(piece.Image, piece.Coordonnee.Ligne - 1);
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }

            if (piece.DeplacementBas)
                if (!(PiecePresente(piece.Coordonnee.Ligne + 1, piece.Coordonnee.Colonne)))
                {
                    Grid.SetRow(piece.Image, piece.Coordonnee.Ligne + 1);
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }

            if (piece.DeplacementGauche)
                if (!(PiecePresente(piece.Coordonnee.Ligne, piece.Coordonnee.Colonne - 1)))
                {
                    Grid.SetColumn(piece.Image, piece.Coordonnee.Colonne - 1);
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }

            if (piece.DeplacementDroite)
                if (!(PiecePresente(piece.Coordonnee.Ligne, piece.Coordonnee.Colonne + 1)))
                {
                    Grid.SetColumn(piece.Image, piece.Coordonnee.Colonne + 1);
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }
        }

        /// <summary>
        /// Méthode permettant de tester s'il y a un élement dans la grille à la position demandée
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool PiecePresente(int row, int column)
        {
            return
                JeuGrid.Children.Cast<FrameworkElement>()
                    .Any(element => (Grid.GetRow(element) == row) && (Grid.GetColumn(element) == column));
        }
    }
}