using System;

namespace TaqNShare.Donnees
{
    /// <summary>
    /// Classe permettant de remplir la liste d'amis facebook
    /// </summary>
    public class AmisFacebook
    {
        #region Propriétés

        public string Id { get; set; }

        public string Nom { get; set; }

        public Uri Image { get; set; }

        #endregion
    }
}
