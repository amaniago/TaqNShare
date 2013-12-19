namespace TaqNShare.Data
{
    public class Parametre
    {
        public int Id { get; set; }
        public int Valeur { get; set; }

        public Parametre(int id, int valeur)
        {
            this.Id = id;
            this.Valeur = valeur;
        }
    }
}
