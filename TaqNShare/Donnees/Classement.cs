namespace TaqNShare.Donnees
{
    /// <summary>
    /// Permet de remplir le classement
    /// </summary>
    public class Classement
    {
        #region Propriétés
        public int Position { get; set; }

        public string Nom { get; set; }

        public string Prenom { get; set; }

        public float ScoreTotal { get; set; }

        #endregion
    }
}
