using System;
using System.Runtime.Serialization;

namespace ServeurTaqnshare.ClasseDeService
{
    /// <summary>
    /// Classe permettant de transmettre la composition d'un défi par le web service
    /// </summary>
    [DataContract(IsReference = true)]
    public class CompositionService
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="composer"></param>
        public CompositionService(Composer composer)
        {
            IdPiece = composer.id_piece;
            IdFiltre = composer.id_filtre;
            IndexPosition = Convert.ToInt32(composer.position_piece);
        }

        [DataMember]
        public int IdPiece { get; set; }
        [DataMember]
        public int IdFiltre { get; set; }
        [DataMember]
        public int IndexPosition { get; set; }
    }
}
