﻿using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;

namespace TaqNShare.Donnees
{
    /// <summary>
    /// Permet de gérer les pièces des parties ou des défis
    /// </summary>
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

        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Piece()
        {
        }

        /// <summary>
        /// Contructeur partie normale
        /// </summary>
        /// <param name="image"></param>
        public Piece(Image image)
        {
            Image = image;
            Id = Convert.ToInt32(image.Name);
            IndexPosition = Id;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out _tailleGrille);
            Ajuster();
        }

        /// <summary>
        /// Constructeur cas defi
        /// </summary>
        /// <param name="image"></param>
        /// <param name="indexPosition"></param>
        public Piece(Image image, int indexPosition)
        {
            Image = image;
            Id = Convert.ToInt32(image.Name);
            IndexPosition = indexPosition;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("TailleGrille", out _tailleGrille);
            Ajuster();
        }

        #endregion

        #region Méthodes

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
        /// Méthode générale permettant de déterminer la position d'une pièce dans la grille
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

        /** Ensemble des méthodes pour déterminer la position d'une pièce - DEBUT **/
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
        /** Ensemble des méthodes pour déterminer la position d'une pièce - FIN **/

        /// <summary>
        /// Méthode permettant de calculer les coordonnées en fonction de l'index de position et de la taille de la grille
        /// Utilisée dans le cas des défis
        /// </summary>
        /// <param name="indexPosition"></param>
        /// <param name="tailleGrille"></param>
        /// <returns></returns>
        public static int[] CalculerCoordonnees(int indexPosition, int tailleGrille)
        {
            int[] coordonnees = new int[2];

            if (tailleGrille == 3)
            {
                switch (indexPosition)
                {
                    case 0:
                        coordonnees[0] = 0;
                        coordonnees[1] = 0;
                        break;
                    case 1:
                        coordonnees[0] = 0;
                        coordonnees[1] = 1;
                        break;
                    case 2:
                        coordonnees[0] = 0;
                        coordonnees[1] = 2;
                        break;
                    case 3:
                        coordonnees[0] = 1;
                        coordonnees[1] = 0;
                        break;
                    case 4:
                        coordonnees[0] = 1;
                        coordonnees[1] = 1;
                        break;
                    case 5:
                        coordonnees[0] = 1;
                        coordonnees[1] = 2;
                        break;
                    case 6:
                        coordonnees[0] = 2;
                        coordonnees[1] = 0;
                        break;
                    case 7:
                        coordonnees[0] = 2;
                        coordonnees[1] = 1;
                        break;
                    case 8:
                        coordonnees[0] = 2;
                        coordonnees[1] = 2;
                        break;
                }
            }
            else if (tailleGrille == 4)
            {
                switch (indexPosition)
                {
                    case 0:
                        coordonnees[0] = 0;
                        coordonnees[1] = 0;
                        break;
                    case 1:
                        coordonnees[0] = 0;
                        coordonnees[1] = 1;
                        break;
                    case 2:
                        coordonnees[0] = 0;
                        coordonnees[1] = 2;
                        break;
                    case 3:
                        coordonnees[0] = 0;
                        coordonnees[1] = 3;
                        break;
                    case 4:
                        coordonnees[0] = 1;
                        coordonnees[1] = 0;
                        break;
                    case 5:
                        coordonnees[0] = 1;
                        coordonnees[1] = 1;
                        break;
                    case 6:
                        coordonnees[0] = 1;
                        coordonnees[1] = 2;
                        break;
                    case 7:
                        coordonnees[0] = 1;
                        coordonnees[1] = 3;
                        break;
                    case 8:
                        coordonnees[0] = 2;
                        coordonnees[1] = 0;
                        break;
                    case 9:
                        coordonnees[0] = 2;
                        coordonnees[1] = 1;
                        break;
                    case 10:
                        coordonnees[0] = 2;
                        coordonnees[1] = 2;
                        break;
                    case 11:
                        coordonnees[0] = 2;
                        coordonnees[1] = 3;
                        break;
                    case 12:
                        coordonnees[0] = 3;
                        coordonnees[1] = 0;
                        break;
                    case 13:
                        coordonnees[0] = 3;
                        coordonnees[1] = 1;
                        break;
                    case 14:
                        coordonnees[0] = 3;
                        coordonnees[1] = 2;
                        break;
                    case 15:
                        coordonnees[0] = 3;
                        coordonnees[1] = 3;
                        break;
                }
            }
            else if (tailleGrille == 5)
            {
                switch (indexPosition)
                {
                    case 0:
                        coordonnees[0] = 0;
                        coordonnees[1] = 0;
                        break;
                    case 1:
                        coordonnees[0] = 0;
                        coordonnees[1] = 1;
                        break;
                    case 2:
                        coordonnees[0] = 0;
                        coordonnees[1] = 2;
                        break;
                    case 3:
                        coordonnees[0] = 0;
                        coordonnees[1] = 3;
                        break;
                    case 4:
                        coordonnees[0] = 0;
                        coordonnees[1] = 4;
                        break;
                    case 5:
                        coordonnees[0] = 1;
                        coordonnees[1] = 0;
                        break;
                    case 6:
                        coordonnees[0] = 1;
                        coordonnees[1] = 1;
                        break;
                    case 7:
                        coordonnees[0] = 1;
                        coordonnees[1] = 2;
                        break;
                    case 8:
                        coordonnees[0] = 1;
                        coordonnees[1] = 3;
                        break;
                    case 9:
                        coordonnees[0] = 1;
                        coordonnees[1] = 4;
                        break;
                    case 10:
                        coordonnees[0] = 2;
                        coordonnees[1] = 0;
                        break;
                    case 11:
                        coordonnees[0] = 2;
                        coordonnees[1] = 1;
                        break;
                    case 12:
                        coordonnees[0] = 2;
                        coordonnees[1] = 2;
                        break;
                    case 13:
                        coordonnees[0] = 2;
                        coordonnees[1] = 3;
                        break;
                    case 14:
                        coordonnees[0] = 2;
                        coordonnees[1] = 4;
                        break;
                    case 15:
                        coordonnees[0] = 3;
                        coordonnees[1] = 0;
                        break;
                    case 16:
                        coordonnees[0] = 3;
                        coordonnees[1] = 1;
                        break;
                    case 17:
                        coordonnees[0] = 3;
                        coordonnees[1] = 2;
                        break;
                    case 18:
                        coordonnees[0] = 3;
                        coordonnees[1] = 3;
                        break;
                    case 19:
                        coordonnees[0] = 3;
                        coordonnees[1] = 4;
                        break;
                    case 20:
                        coordonnees[0] = 4;
                        coordonnees[1] = 0;
                        break;
                    case 21:
                        coordonnees[0] = 4;
                        coordonnees[1] = 1;
                        break;
                    case 22:
                        coordonnees[0] = 4;
                        coordonnees[1] = 2;
                        break;
                    case 23:
                        coordonnees[0] = 4;
                        coordonnees[1] = 3;
                        break;
                    case 24:
                        coordonnees[0] = 4;
                        coordonnees[1] = 4;
                        break;
                }
            }

            return coordonnees;
        }

        #endregion

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
