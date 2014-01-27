using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TaqNShare.Data
{
    sealed class Partie : INotifyPropertyChanged
    {

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
                    _stopWatch.Start();
                    _timer.Start();
                }

                _nombreDeplacement = value;
                OnPropertyChanged();
            }
        }

        public string TimeElapsed { get; set; }

        private readonly Stopwatch _stopWatch;
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// Contructeur
        /// </summary>
        public Partie(int tailleGrille)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(delegate
            {
                TimeElapsed = _stopWatch.Elapsed.ToString("mm\\:ss\\.ff"); // Format as you wish
                PropertyChanged(this, new PropertyChangedEventArgs("TimeElapsed"));
            });
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _stopWatch = new Stopwatch();

            TailleGrille = tailleGrille;
            ListePieces = new List<Piece>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
