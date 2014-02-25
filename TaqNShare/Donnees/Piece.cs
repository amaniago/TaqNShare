using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;

namespace TaqNShare.Donnees
{
    class Piece
    {
        #region propriétés

        public Image Image { get; set; }
        public int Id { get; set; }
        private string Position { get; set; }
        public CoordonneePiece Coordonnee { get; set; }
        public bool DeplacementHaut { get; set; }
        public bool DeplacementBas { get; set; }
        public bool DeplacementGauche { get; set; }
        public bool DeplacementDroite { get; set; }
        public int IndexPosition { get; set; }
        public int IdFiltre { get; set; }

        private readonly int _tailleGrille;

        #endregion propriétés

        /// <summary>
        /// Constructeur par défault
        /// </summary>
        public Piece()
        {

        }

        /// <summary>
        /// Contructeur
        /// </summary>
        public Piece(Image image)
        {
            Image = image;
            Id = Convert.ToInt32(image.Name);
            IndexPosition = Id;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out _tailleGrille);
            Ajuster();
        }

        /// <summary>
        /// Méthode permettant d'ajuster les attributs d'une pièce suivant sa position dans la grille
        /// </summary>
        internal void Ajuster()
        {
            Coordonnee = new CoordonneePiece(Image);
            Position = DeterminerPosition(Coordonnee);
            DeterminerDeplacements(Position);
        }

        /// <summary>
        /// Méthode permettant de déterminer les déplacements possibles d'une pièce suivant sa position
        /// </summary>
        /// <param name="position"></param>
        private void DeterminerDeplacements(string position)
        {
            switch (position)
            {
                case "BordHaut":
                    DeplacementHaut = false;
                    DeplacementBas = true;
                    DeplacementGauche = true;
                    DeplacementDroite = true;
                    break;

                case "BordBas":
                    DeplacementHaut = true;
                    DeplacementBas = false;
                    DeplacementGauche = true;
                    DeplacementDroite = true;
                    break;

                case "BordGauche":
                    DeplacementHaut = true;
                    DeplacementBas = true;
                    DeplacementGauche = false;
                    DeplacementDroite = true;
                    break;

                case "BordDroit":
                    DeplacementHaut = true;
                    DeplacementBas = true;
                    DeplacementGauche = true;
                    DeplacementDroite = false;
                    break;

                case "CoinHautGauche":
                    DeplacementHaut = false;
                    DeplacementBas = true;
                    DeplacementGauche = false;
                    DeplacementDroite = true;
                    break;

                case "CoinHautDroit":
                    DeplacementHaut = false;
                    DeplacementBas = true;
                    DeplacementGauche = true;
                    DeplacementDroite = false;
                    break;

                case "CoinBasGauche":
                    DeplacementHaut = true;
                    DeplacementBas = false;
                    DeplacementGauche = false;
                    DeplacementDroite = true;
                    break;

                case "CoinBasDroit":
                    DeplacementHaut = true;
                    DeplacementBas = false;
                    DeplacementGauche = true;
                    DeplacementDroite = false;
                    break;

                case "Milieu":
                    DeplacementHaut = true;
                    DeplacementBas = true;
                    DeplacementGauche = true;
                    DeplacementDroite = true;
                    break;
            }
        }

        /// <summary>
        /// Méthode permettant de déterminer la position d'une pièce dans la grille
        /// </summary>
        /// <param name="coordonneePiece"></param>
        /// <returns></returns>
        private string DeterminerPosition(CoordonneePiece coordonneePiece)
        {
            if (EstSurBordHaut(coordonneePiece)) return "BordHaut";
            if (EstSurBordBas(coordonneePiece)) return "BordBas";
            if (EstSurBordGauche(coordonneePiece)) return "BordGauche";
            if (EstSurBordDroit(coordonneePiece)) return "BordDroit";
            if (EstSurCoinHautGauche(coordonneePiece)) return "CoinHautGauche";
            if (EstSurCoinHautDroit(coordonneePiece)) return "CoinHautDroit";
            if (EstSurCoinBasGauche(coordonneePiece)) return "CoinBasGauche";
            if (EstSurCoinBasDroit(coordonneePiece)) return "CoinBasDroit";

            return "Milieu";
        }


        private bool EstSurBordHaut(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Ligne == 0) && (coordonneeImageCliquee.Colonne != 0) &&
                   (coordonneeImageCliquee.Colonne != _tailleGrille - 1);
        }

        private bool EstSurBordBas(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Ligne == _tailleGrille - 1) &&
                   (coordonneeImageCliquee.Colonne != 0) &&
                   (coordonneeImageCliquee.Colonne != _tailleGrille - 1);
        }

        private bool EstSurBordGauche(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == 0) && (coordonneeImageCliquee.Ligne != 0) &&
                   (coordonneeImageCliquee.Ligne != _tailleGrille - 1);
        }

        private bool EstSurBordDroit(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == _tailleGrille - 1) &&
                   (coordonneeImageCliquee.Ligne != 0) && (coordonneeImageCliquee.Ligne != _tailleGrille - 1);
        }

        private bool EstSurCoinHautGauche(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == 0) && (coordonneeImageCliquee.Ligne == 0);
        }

        private bool EstSurCoinHautDroit(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == _tailleGrille - 1) &&
                   (coordonneeImageCliquee.Ligne == 0);
        }

        private bool EstSurCoinBasGauche(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == 0) &&
                   (coordonneeImageCliquee.Ligne == _tailleGrille - 1);
        }

        private bool EstSurCoinBasDroit(CoordonneePiece coordonneeImageCliquee)
        {
            return (coordonneeImageCliquee.Colonne == _tailleGrille - 1) &&
                   (coordonneeImageCliquee.Ligne == _tailleGrille - 1);
        }

        /// <summary>
        /// Structure permettant de stocker les cooordonnées d'une pièce
        /// </summary>
        internal struct CoordonneePiece
        {
            public int Ligne { get; set; }
            public int Colonne { get; set; }

            public CoordonneePiece(FrameworkElement imageCliquee)
                : this()
            {
                Ligne = Grid.GetRow(imageCliquee);
                Colonne = Grid.GetColumn(imageCliquee);
            }
        }
    }
}
