using PokemonReviewApp.Models;
using System.Collections.Generic;

namespace PokemonReviewApp.Interfaces
{
    public interface IFoodTypeRepository
    {
        ICollection<FoodType> GetFoodTypes();
        FoodType GetFoodType(int foodTypeId);
        bool FoodTypeExists(int foodTypeId);
        bool CreateFoodType(FoodType foodType);
        bool UpdateFoodType(FoodType foodType);
        bool SoftDeleteFoodType(int foodTypeId);
        bool Save();
    }
}
