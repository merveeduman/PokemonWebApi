namespace PokemonReviewApp.Dto
{
    public class FoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string Type { get; set; } // Bu alan kaldırılabilir
        public int FoodTypeId { get; set; }  // FoodTypeId eklendi
        public decimal Price { get; set; }
    }

}
