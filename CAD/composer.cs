//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CAD
{
    using System;
    using System.Collections.Generic;
    
    public partial class composer
    {
        public string id_piece { get; set; }
        public string id_filtre { get; set; }
        public int id_defi { get; set; }
        public Nullable<decimal> position_piece { get; set; }
    
        public virtual defi defi { get; set; }
        public virtual filtre filtre { get; set; }
        public virtual piece piece { get; set; }
    }
}
