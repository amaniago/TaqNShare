using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;

namespace TaqNShare.Data
{
    class Piece
    {
        #region : propriétés

        public Image Image { get; set; }
        public int Id { get; set; }
        public string Position { get; set; }
        public CoordonneePiece Coordonnee { get; set; }
        public bool DeplacementHaut { get; set; }
        public bool DeplacementBas { get; set; }
        public bool DeplacementGauche { get; set; }
        public bool DeplacementDroite { get; set; }

        #endregion : propriétés

        private readonly int _tailleGrille;

        /// <summary>
        /// Contructeur
        /// </summary>
        public Piece(Image image)
        {
            Image = image;
            Coordonnee = new CoordonneePiece(image);
            Id = Convert.ToInt32(image.Name);
            Position = DeterminerPosition(Coordonnee);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out _tailleGrille);
            DeterminerDeplacements(Position);
        }

        private void Ajuster()
        {
            
        }

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
