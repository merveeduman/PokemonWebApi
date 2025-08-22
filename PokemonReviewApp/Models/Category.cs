  namespace PokemonReviewApp.Models
{
    public class Category : BaseEntity
    {

        public int Id { get; set; }
        public string Name { get; set; }
       

        public ICollection<PokemonCategory> PokemonCategories { get; set; }
    }
}
