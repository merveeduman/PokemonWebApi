using PokemonReviewApp.Models;
using System.Collections.Generic;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonFoodRepository
    {
        ICollection<PokemonFood> GetPokemonFoods();
        PokemonFood GetPokemonFood(int pokemonId, int foodId);
        bool PokemonFoodExists(int pokemonId, int foodId);
        bool CreatePokemonFood(PokemonFood pokemonFood);
        bool UpdatePokemonFood(PokemonFood pokemonFood);
        bool DeletePokemonFood(PokemonFood pokemonFood);
        bool SoftDeletePokemonFood(int pokemonId, int foodId);
        bool PokemonExists(int pokemonId);
        bool FoodExists(int foodId);
        bool Save();
    }
}
