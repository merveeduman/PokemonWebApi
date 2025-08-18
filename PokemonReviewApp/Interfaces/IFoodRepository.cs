using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IFoodRepository
    {
        ICollection<Food> GetActiveFoods();
        Food GetFood(int id);
        Food GetFoodByName(string name);
        bool CreateFood(Food food);
        bool UpdateFood(Food food);
        bool SoftDeleteFood(int foodId);
        bool FoodExists(int foodId);

        ICollection<Pokemon> GetPokemonsOfFood(int foodId);
        FoodType GetFoodTypeOfFood(int foodId);
      
        bool Save();
    }

}
