using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;
using Nokia.Graphics.Imaging;

namespace TaqNShare.Data
{
    /// <summary>
    /// Permet de stocker l'image selectionnée par l'utilisateur
    /// </summary>
    class Photo
    {
        public WriteableBitmap PhotoSelectionne { get; set; }
        public BufferImageSource PhotoBuffer { get; set; }

        /// <summary>
        /// Constructeur de la classe photo qui permet de transformer l'image sélectionnée en Buffer pour l'application des filtres
        /// </summary>
        /// <param name="photoSelectionne"></param>
        public Photo(WriteableBitmap photoSelectionne)
        {
            PhotoSelectionne = photoSelectionne;

            var fileStream = new MemoryStream();
            photoSelectionne.SaveJpeg(fileStream, 750, 450, 100, 100);
            fileStream.Seek(0, SeekOrigin.Begin);
            IBuffer buffer = fileStream.GetWindowsRuntimeBuffer();
            PhotoBuffer = new BufferImageSource(buffer);
        }
    }
}
