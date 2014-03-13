using Nokia.Graphics.Imaging;

namespace TaqNShare.Donnees
{
    /// <summary>
    /// Permet de remplir la liste de filtres dans laquelle sont choisis les filtres des parties
    /// </summary>
    class Filtre
    {
        #region Propriétés
        public int Id { get; set; }
        public IFilter Type { get; set; }
        #endregion

        public Filtre(int id, IFilter type)
        {
            Id = id;
            Type = type;
        }
    }
}
