using Nokia.Graphics.Imaging;

namespace TaqNShare.Donnees
{
    class Filtre
    {
        public int Id { get; set; }
        public IFilter Type { get; set; }

        public Filtre(int id, IFilter type)
        {
            Id = id;
            Type = type;
        }
    }
}
