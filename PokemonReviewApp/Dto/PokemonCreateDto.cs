namespace PokemonReviewApp.Dto
{
    public class PokemonCreateDto
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        // Foreign key alanları
        public int OwnerId { get; set; }
        public int CategoryId { get; set; }
    }
}
