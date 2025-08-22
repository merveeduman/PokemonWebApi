namespace PokemonReviewApp.Models
{
    public class Reviewer : BaseEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public bool IsDeleted { get; set; } = false;   //  soft delete için ekledim
    }
}