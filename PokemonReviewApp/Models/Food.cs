namespace PokemonReviewApp.Models
{
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int FoodTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public FoodType FoodType { get; set; }

        public ICollection<PokemonFood> PokemonFoods { get; set; }
    }

}
