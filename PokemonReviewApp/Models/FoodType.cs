using System.ComponentModel.DataAnnotations.Schema;

namespace PokemonReviewApp.Models
{
    [Table("FoodType")]
    public class FoodType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Food> Foods { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
