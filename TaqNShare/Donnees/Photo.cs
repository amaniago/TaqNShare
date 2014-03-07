using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;
using Nokia.Graphics.Imaging;

namespace TaqNShare.Donnees
{
    /// <summary>
    /// Permet de stocker l'image selectionnée par l'utilisateur
    /// </summary>
    class Photo
    {
        #region Propriétés

        public WriteableBitmap PhotoSelectionne { get; set; }
        public BufferImageSource PhotoBuffer { get; set; }

        #endregion

        /// <summary>
        /// Constructeur de la classe photo qui permet de transformer l'image sélectionnée en Buffer pour l'application des filtres
        /// </summary>
        /// <param name="photoSelectionne"></param>
        /// <param name="largeurPiece"></param>
        /// <param name="hauteurPiece"></param>
        public Photo(WriteableBitmap photoSelectionne, int largeurPiece, int hauteurPiece)
        {
            PhotoSelectionne = photoSelectionne;

            var fileStream = new MemoryStream();
            photoSelectionne.SaveJpeg(fileStream, largeurPiece, hauteurPiece, 100, 100);
            fileStream.Seek(0, SeekOrigin.Begin);
            IBuffer buffer = fileStream.GetWindowsRuntimeBuffer();
            PhotoBuffer = new BufferImageSource(buffer);
        }

        #region Méthodes

        /// <summary>
        /// Permet de convertir une image en tableau de byte pour le transfert par le web service
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(WriteableBitmap photo)
        {
            MemoryStream ms = new MemoryStream();
            photo.SaveJpeg(ms, photo.PixelWidth, photo.PixelHeight, 0, 100);
            return ms.ToArray();
        }

        /// <summary>
        /// Permet de décoder un tableau de byte en image lors du renvoi de l'image par le web service lors d'un défi
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static BitmapImage DecodeImage(byte[] array)
        {
            Stream stream = new MemoryStream(array);
            BitmapImage image = new BitmapImage();

            image.SetSource(stream);

            return image;
        }

        #endregion
    }
}
