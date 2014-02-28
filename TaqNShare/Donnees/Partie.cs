using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TaqNShare.Donnees
{
    sealed class Partie : INotifyPropertyChanged
    {
        #region propriétés

        public int Score { get; set; }
        public List<Piece> ListePieces { get; private set; }
        public List<Piece> ListePiecesInitale { get; set; }
        public WriteableBitmap Photo { get; set; }

        public int TailleGrille { get; set; }
        public int NombreFiltre { get; set; }

        private int _nombreDeplacement = 0;
        public int NombreDeplacement
        {

            get
            {
                return _nombreDeplacement;
            }

            set
            {
                if (value == 1)
                {
                    StopWatch.Start();
                    _timer.Start();
                }

                _nombreDeplacement = value;
                OnPropertyChanged();
            }
        }

        public string TimeElapsed { get; set; }


        public readonly Stopwatch StopWatch;
        private readonly DispatcherTimer _timer;

        #endregion propriétés

        /// <summary>
        /// Contructeur
        /// </summary>
        public Partie(int tailleGrille, int nombreFiltre)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(delegate
            {
                TimeElapsed = StopWatch.Elapsed.ToString("mm\\:ss"); // Format as you wish
                PropertyChanged(this, new PropertyChangedEventArgs("TimeElapsed"));
            });
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            StopWatch = new Stopwatch();

            
            TailleGrille = tailleGrille;

            NombreFiltre = nombreFiltre;

            ListePieces = new List<Piece>();
        }

        public Partie()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(delegate
            {
                TimeElapsed = StopWatch.Elapsed.ToString("mm\\:ss"); // Format as you wish
                PropertyChanged(this, new PropertyChangedEventArgs("TimeElapsed"));
            });
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            StopWatch = new Stopwatch();

            ListePieces = new List<Piece>();
        }


        //Permet le binding des propriétés de la classe
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Méthode permettant de détecter la fin du jeu
        /// </summary>
        /// <returns></returns>
        public bool DetecterFinJeu()
        {
            //return true;
            return ListePieces.All(piece => piece.Id == piece.IndexPosition);
        }

        /// <summary>
        /// Méthode permettant de calculer le score de l'utilisateur
        /// </summary>
        public void CalculerScore()
        {
            //Score = temps de résolution * 0,6 + nombre de déplacements + Malus facilité 
            //(Le score le plus faible est le meilleur)

            int malusFacilite = 0;
            switch (TailleGrille)
            {
                case 3:
                    malusFacilite += 1000;
                    break;

                case 4:
                    malusFacilite += 500;
                    break;
            }

            switch (NombreFiltre)
            {
                case 0:
                    malusFacilite += 1000;
                    break;

                case 1:
                    malusFacilite += 500;
                    break;
            }

            Score = Convert.ToInt32(Math.Round(((StopWatch.ElapsedMilliseconds / 1000.0) * 0.6) + NombreDeplacement + malusFacilite));
        }
    }
}
