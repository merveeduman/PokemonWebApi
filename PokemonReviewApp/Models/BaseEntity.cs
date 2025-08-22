namespace PokemonReviewApp.Models
{
    public class BaseEntity
    {
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedUserDateTime { get; set; }
        public bool IsDeleted { get; set; }   //soft delete için deneme bu şekilde
    }
}
