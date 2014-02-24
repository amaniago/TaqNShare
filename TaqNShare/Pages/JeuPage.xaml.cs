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
using Nokia.Graphics.Imaging;
using TaqNShare.Donnees;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace TaqNShare.Pages
{
    /// <summary>
    /// Page de jeu :
    ///     -Préparation de l'image
    ///     -Déroulement du jeu
    /// </summary>
    public partial class JeuPage
    {
        private Partie _partieEnCours;
        private readonly ProgressIndicator _indicator;

        public JeuPage()
        {
            InitializeComponent();

            //Barre de progression qui s'affiche lors du chargement de la partie
            _indicator = new ProgressIndicator
            {
                IsVisible = true,
                IsIndeterminate = true,
                Text = "Préparation de l'image en cours..."
            };
            SystemTray.SetProgressIndicator(this, _indicator);

            //Appel à la méthode de préparation de l'image
            Loaded += PreparerImage;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int tailleGrille;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out tailleGrille);

            int nombreFiltre;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("IndexFiltre", out nombreFiltre);
            //Création de la partie
            _partieEnCours = new Partie(tailleGrille, nombreFiltre);
            //Spécification du dataContext pour le Binding
            DataContext = _partieEnCours;

            base.OnNavigatedTo(e);
        }


        /// <summary>
        /// Méthode permettant le découpage de l'image, l'application des filtres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private async void PreparerImage(object sender, RoutedEventArgs routedEventArgs)
        {
            int hauteurPiece = 244;
            int largeurPiece = 150;

            //Initialisation de la taille des pièce suivant le nombre de découpage choisi par l'utilisateur
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
            WriteableBitmap imageSelectionne = (WriteableBitmap)PhoneApplicationService.Current.State["photo"];;
            _partieEnCours.Photo = imageSelectionne;
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

            List<IFilter> listeFiltre = new List<IFilter>
                                        {
                                            new AntiqueFilter(),
                                            new CartoonFilter(),
                                            new MagicPenFilter()
                                            
                                            //new ChromaKeyFilter()
                                            //new ExposureFilter()
                                            //new MonoColorFilter()
                                            //new ColorBoostFilter()
                                            //new CurvesFilter()
                                        };



            List<IFilter> listeFiltrePartie = new List<IFilter>();

            for (int i = 0; i < _partieEnCours.NombreFiltre; i++)
            {
                Random random = new Random();
                int indexRandom = random.Next(listeFiltre.Count);
                listeFiltrePartie.Add(listeFiltre[indexRandom]);
                listeFiltre.Remove(listeFiltre[indexRandom]);
            }

            //Préparation de l'image 
            int compteur = 0;
            for (int i = 0; i < _partieEnCours.TailleGrille; i++)
            {
                for (int j = 0; j < _partieEnCours.TailleGrille; j++)
                {
                    if (!(i == _partieEnCours.TailleGrille - 1 && j == _partieEnCours.TailleGrille - 1))
                    {
                        //Découpage de la photo : TODO explication
                        Photo photoDecoupe = new Photo(imageSelectionne.Crop(i * largeurPiece, j * hauteurPiece, largeurPiece, hauteurPiece), largeurPiece, hauteurPiece);

                        //TODO Application des filtres
                        if (_partieEnCours.NombreFiltre != 0)
                        {
                            FilterEffect filterEffect = new FilterEffect(photoDecoupe.PhotoBuffer);
                            Random random2 = new Random();
                            int indexRandom2 = random2.Next(_partieEnCours.NombreFiltre);
                            filterEffect.Filters = new[] { listeFiltrePartie[indexRandom2] };

                            // Render the image to a WriteableBitmap.
                            var renderer = new WriteableBitmapRenderer(filterEffect, photoDecoupe.PhotoSelectionne);
                            photoDecoupe.PhotoSelectionne = await renderer.RenderAsync();
                        }

                        Image image = new Image
                        {
                            Name = compteur.ToString(CultureInfo.InvariantCulture),
                            Source = photoDecoupe.PhotoSelectionne
                        };
                        Grid.SetRow(image, j);
                        Grid.SetColumn(image, i);
                        JeuGrid.Children.Add(image);
                        Piece piece = new Piece(image);
                        _partieEnCours.ListePieces.Add(piece);
                        compteur++;
                    }
                }
            }

            //Appel à la méthode de mélange
            Melange();

            foreach (var piece in _partieEnCours.ListePieces)
            {
                piece.Image.Tap += ImageTap;
            }
            
        }

        /// <summary>
        /// Méthode permettant le mélange des pièces*
        /// </summary>
        private void Melange()
        {
            Random random = new Random();

            //Début boucle pour effectuer 100 déplacements
            for (int i = 0; i < 250; i++)
            {
                //Récupération des pièces déplacables
                List<Piece> listePiecesDeplacables = _partieEnCours.ListePieces.Where(EstDeplacable).ToList();

                //Prise d'une pièce au hasard dans la liste des pièces déplacables
                int indexRandom = random.Next(listePiecesDeplacables.Count);
                var pieceADeplacer = listePiecesDeplacables[indexRandom];

                //Suppression de la pièce à déplacer de la liste
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

                //Déplacement de la pièce
                DeplacementSuivantPosition(pieceADeplacer, true);

                //Calcul des nouveaux paramètres après déplacement
                pieceADeplacer.Ajuster();

                //Ajout de la pièce à la nouvelle position avec les nouveaux paramètres dans la liste des pièces de la partie en cours
                _partieEnCours.ListePieces.Add(pieceADeplacer);

            }//Fin boucle   

            //On cache la barre de progression à la fin du mélange
            _indicator.IsIndeterminate = false;
            _indicator.IsVisible = false;
        }

        /// <summary>
        /// Méthode appelée lors de l'appui sur une image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageTap(object sender, GestureEventArgs e)
        {
            //Récupération de la pièce correspondante à l'image cliquée
            int tailleListe = _partieEnCours.ListePieces.Count;
            Piece piececlique = new Piece();

            for (int index = 0; index < tailleListe; index++)
            {
                var piece = _partieEnCours.ListePieces[index];
                if (piece.Image.Equals(sender as Image))
                {
                    piececlique = piece;
                    _partieEnCours.ListePieces.Remove(piece);
                    tailleListe--;
                }
            }

            //Déplacement de la pièce
            DeplacementSuivantPosition(piececlique, false);

            //Calcul des nouveaux paramètres après déplacement
            piececlique.Ajuster();

            //Ajout de la pièce à la nouvelle position avec les nouveaux paramètres dans la liste des pièces de la partie en cours
            _partieEnCours.ListePieces.Add(piececlique);

            //Détection de la fin de la partie (Image remise dans l'ordre)
            if (_partieEnCours.DetecterFinJeu())
            {
                //Arrêt du chrono
                _partieEnCours.StopWatch.Stop();
                _partieEnCours.CalculerScore();
                //Stockage de la partie pour la passer à la page suivante
                PhoneApplicationService.Current.State["partie"] = _partieEnCours;
                NavigationService.Navigate(new Uri("/Pages/JeuTerminePage.xaml", UriKind.Relative));
            }

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
                    piece.IndexPosition -= 1;
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }

            if (piece.DeplacementBas)
                if (!(PiecePresente(piece.Coordonnee.Ligne + 1, piece.Coordonnee.Colonne)))
                {
                    Grid.SetRow(piece.Image, piece.Coordonnee.Ligne + 1);
                    piece.IndexPosition += 1;
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }

            if (piece.DeplacementGauche)
                if (!(PiecePresente(piece.Coordonnee.Ligne, piece.Coordonnee.Colonne - 1)))
                {
                    Grid.SetColumn(piece.Image, piece.Coordonnee.Colonne - 1);
                    piece.IndexPosition -= _partieEnCours.TailleGrille;
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }

            if (piece.DeplacementDroite)
                if (!(PiecePresente(piece.Coordonnee.Ligne, piece.Coordonnee.Colonne + 1)))
                {
                    Grid.SetColumn(piece.Image, piece.Coordonnee.Colonne + 1);
                    piece.IndexPosition += _partieEnCours.TailleGrille;
                    if (!casMelange)
                        _partieEnCours.NombreDeplacement++;
                }
        }

        /// <summary>
        /// Méthode permttant de dire si une pièce est déplacable
        /// </summary>
        /// <param name="piece"></param>
        /// <returns>True si la pièce est déplacable</returns>
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