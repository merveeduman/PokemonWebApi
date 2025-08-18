namespace PokemonReviewApp.Models
{
    public class PokemonFood
    {
        public int PokemonId { get; set; }
        public Pokemon Pokemon { get; set; }

        public int FoodId { get; set; }
        public Food Food { get; set; }
        public bool IsDeleted { get; set; }

      
    }

}
