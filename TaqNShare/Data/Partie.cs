using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace TaqNShare.Data
{
    sealed class Partie : INotifyPropertyChanged
    {
        public int Score { get; set; }
        public List<Piece> ListePieces { get; private set; }
        public Photo PhotoSelectionne { get; set; }

        public int TailleGrille { get; set; }

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

        /// <summary>
        /// Contructeur
        /// </summary>
        public Partie(int tailleGrille)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(delegate
            {
                TimeElapsed = StopWatch.Elapsed.ToString("mm\\:ss\\.ff"); // Format as you wish
                PropertyChanged(this, new PropertyChangedEventArgs("TimeElapsed"));
            });
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            StopWatch = new Stopwatch();

            TailleGrille = tailleGrille;
            ListePieces = new List<Piece>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool DetecterFinJeu()
        {
            return ListePieces.All(piece => piece.Id == piece.IndexPosition);
        }

        public void CalculerScore()
        {
            //Score = temps de résolution * 0,6 + nombre de déplacements + Malus facilité 
            //(Le score le plus faible est le meilleur)
             
        }
    }
}
