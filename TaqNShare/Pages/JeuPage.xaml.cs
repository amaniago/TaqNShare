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
using TaqNShare.TaqnshareReference;
using Filtre = TaqNShare.Donnees.Filtre;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using Piece = TaqNShare.Donnees.Piece;

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
        //Liste des filtres disponibles dans le sdk
        readonly List<Filtre> _listeFiltre = new List<Filtre>
                                        {
                                            new Filtre(1, new AntiqueFilter()),
                                            new Filtre(2, new CartoonFilter()),
                                            new Filtre(3, new MagicPenFilter()),
                                            
                                            //new ChromaKeyFilter()
                                            //new ExposureFilter()
                                            //new MonoColorFilter()
                                            //new ColorBoostFilter()
                                            //new CurvesFilter()
                                        };
        int _hauteurPiece = 244;
        int _largeurPiece = 150;

        private bool _casDefi;

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

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string casDefiQuery;
            if (NavigationContext.QueryString.TryGetValue("casDefi", out casDefiQuery))
            {
                _casDefi = Convert.ToBoolean(casDefiQuery);
            }
            //Appel à la méthode de préparation de l'image
            if (!_casDefi)
                PreparerImage();
            else
                PreparerImageDefi();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Méthode permettant le découpage de l'image, l'application des filtres
        /// </summary>
        private async void PreparerImage()
        {
            int tailleGrille;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out tailleGrille);

            int nombreFiltre;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("IndexFiltre", out nombreFiltre);
            //Création de la partie
            _partieEnCours = new Partie(tailleGrille, nombreFiltre);
            //Spécification du dataContext pour le Binding
            DataContext = _partieEnCours; 

            //Création de la grille
            CreerGrille(_partieEnCours.TailleGrille);

            //Récupération de la UtilisateurImage
            WriteableBitmap imageSelectionne = (WriteableBitmap)PhoneApplicationService.Current.State["photo"];
            _partieEnCours.Photo = imageSelectionne;

            //On choisit des filtres au hasard dans la liste du sdk pour la partie et suivant les paramètres choisis par l'utilisateur
            List<Filtre> listeFiltrePartie = new List<Filtre>();
            for (int i = 0; i < _partieEnCours.NombreFiltre; i++)
            {
                Random random = new Random();
                int indexRandom = random.Next(_listeFiltre.Count);
                listeFiltrePartie.Add(_listeFiltre[indexRandom]);
                _listeFiltre.Remove(_listeFiltre[indexRandom]);
            }

            //Préparation de l'image 
            int compteur = 0;
            for (int i = 0; i < _partieEnCours.TailleGrille; i++)
            {
                for (int j = 0; j < _partieEnCours.TailleGrille; j++)
                {
                    if (!(i == _partieEnCours.TailleGrille - 1 && j == _partieEnCours.TailleGrille - 1))
                    {
                        //Découpage de la UtilisateurImage : TODO explication
                        Photo photoDecoupe = new Photo(imageSelectionne.Crop(i * _largeurPiece, j * _hauteurPiece, _largeurPiece, _hauteurPiece), _largeurPiece, _hauteurPiece);

                        //Application des filtres
                        Random random2 = new Random();
                        int indexRandom2 = random2.Next(_partieEnCours.NombreFiltre);

                        if (_partieEnCours.NombreFiltre != 0)
                        {
                            FilterEffect filterEffect = new FilterEffect(photoDecoupe.PhotoBuffer);
                            filterEffect.Filters = new[] { listeFiltrePartie[indexRandom2].Type };
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

                        piece.IdFiltre = _partieEnCours.NombreFiltre != 0 ? listeFiltrePartie[indexRandom2].Id : 0;

                        _partieEnCours.ListePieces.Add(piece);
                        compteur++;
                    }
                }
            }

            //Appel à la méthode de mélange
            Melange();
            _partieEnCours.ListePiecesInitale = new List<Piece>();

            foreach (var piece in _partieEnCours.ListePieces)
            {
                piece.Image.Tap += ImageTap;

                //Copie de la piece dans un nouvel objet pour éviter de modifier la valeur initiale de la piece
                Piece pieceInitiale = new Piece
                                      {
                                          Id = piece.Id,
                                          Image = piece.Image,
                                          Coordonnee = piece.Coordonnee,
                                          DeplacementHaut = piece.DeplacementHaut,
                                          DeplacementBas = piece.DeplacementBas,
                                          DeplacementGauche = piece.DeplacementGauche,
                                          DeplacementDroite = piece.DeplacementDroite,
                                          IndexPosition = piece.IndexPosition,
                                          IdFiltre = piece.IdFiltre
                                      };

                //Ajout de l'objet dans la liste initiale utilisée ensuite pour créer le défi
                _partieEnCours.ListePiecesInitale.Add(pieceInitiale);
            }
        }

        private async void PreparerImageDefi()
        {
            //Récupération du défi
            DefiService defi = (DefiService)PhoneApplicationService.Current.State["defi"];

            _partieEnCours = new Partie(defi.NombreFiltre);
            //Spécification du dataContext pour le Binding
            DataContext = _partieEnCours; 

            //Calcul de la taille de la grille en fonction
            
            switch (defi.Composition.Count)
            {
                case 8 :
                    _partieEnCours.TailleGrille = 3;
                    break;
                case 15 :
                    _partieEnCours.TailleGrille = 4;
                    break;
                case 24 :
                    _partieEnCours.TailleGrille = 5;
                    break;
            }

            CreerGrille(_partieEnCours.TailleGrille);

            int compteur = 0;
            for (int i = 0; i < _partieEnCours.TailleGrille; i++)
            {
                for (int j = 0; j < _partieEnCours.TailleGrille; j++)
                {
                    if (!(i == _partieEnCours.TailleGrille - 1 && j == _partieEnCours.TailleGrille - 1))
                    {
                        //Découpage de la UtilisateurImage
                        Photo photoDecoupe = new Photo(new WriteableBitmap(Photo.DecodeImage(defi.ImageDefi)).Crop(i * _largeurPiece, j * _hauteurPiece, _largeurPiece, _hauteurPiece), _largeurPiece, _hauteurPiece);

                        
                        Composition composer = null;

                        foreach (var c in defi.Composition)
                        {
                            if (c.IdPiece == compteur)
                                composer = c;
                        }

                        if (defi.Composition[compteur].IdFiltre != 0)
                        {
                            FilterEffect filterEffect = new FilterEffect(photoDecoupe.PhotoBuffer);
                            
                            Filtre filtre = null;
                            foreach (var filtre1 in _listeFiltre)
                            {
                                if (composer != null && filtre1.Id == composer.IdFiltre)
                                {
                                    filtre = filtre1;
                                }
                            }

                            if (filtre != null) filterEffect.Filters = new[] { filtre.Type };
                            var renderer = new WriteableBitmapRenderer(filterEffect, photoDecoupe.PhotoSelectionne);
                            photoDecoupe.PhotoSelectionne = await renderer.RenderAsync();
                        }

                        Image image = new Image
                        {
                            Name = compteur.ToString(CultureInfo.InvariantCulture),
                            Source = photoDecoupe.PhotoSelectionne
                        };

                        //TODO Determiner les coordonees en fonction de l'index
                        int[] coordonnees = Piece.CalculerCoordonnees(composer.IndexPosition, _partieEnCours.TailleGrille);

                        Grid.SetColumn(image, coordonnees[0]);
                        Grid.SetRow(image, coordonnees[1]);
                        JeuGrid.Children.Add(image);
                        Piece piece = new Piece(image, composer.IndexPosition);
                        
                        _partieEnCours.ListePieces.Add(piece);

                        compteur++;
                    }
                }
            }

            foreach (var piece in _partieEnCours.ListePieces)
            {
                piece.Image.Tap += ImageTap;
            }

            _indicator.IsIndeterminate = false;
            _indicator.IsVisible = false;
        }

        /// <summary>
        /// Méthode permettant de creer la grille suivant la taille de la grille chosie par l'utilisateur
        /// </summary>
        private void CreerGrille(int tailleGrille)
        {
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

            //Initialisation de la taille des pièce suivant le nombre de découpage choisi par l'utilisateur
            switch (tailleGrille)
            {
                case 0:
                    if (!_casDefi)
                        _partieEnCours.TailleGrille = 3;
                    break;

                case 4:
                    _hauteurPiece = 183;
                    _largeurPiece = 112;
                    break;

                case 5:
                    _hauteurPiece = 146;
                    _largeurPiece = 90;
                    break;
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
                NavigationService.Navigate(_casDefi
                    ? new Uri("/Pages/DefiTerminePage.xaml", UriKind.Relative)
                    : new Uri("/Pages/JeuTerminePage.xaml", UriKind.Relative));
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