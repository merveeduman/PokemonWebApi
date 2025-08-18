  namespace PokemonReviewApp.Models
{
    public class Category
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }   //soft delete için deneme bu şekilde

        public ICollection<PokemonCategory> PokemonCategories { get; set; }
    }
}
