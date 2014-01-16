namespace TaqNShare.Data
{
    public class Parametre
    {
        public int Id { get; set; }
        public int Valeur { get; set; }
        public int TailleGrille { get; set; }

        public Parametre(int id, int valeur, int tailleGrille)
        {
            Id = id;
            Valeur = valeur;
            TailleGrille = tailleGrille;
        }
    }
}
