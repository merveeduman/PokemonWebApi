using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
      //  ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        Pokemon GetPokemonTrimToUpper(PokemonDto pokemonCreate);
        decimal GetPokemonRating(int pokeId);
        bool PokemonExists(int pokeId);
        bool CreatePokemon(int ownerId, int categoryId, int foodId, Pokemon pokemon);

        bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        //   bool DeletePokemon(Pokemon pokemon);
        bool SoftDeletePokemon(int id);


        List<Pokemon> GetActivePokemons(); // yeni ekledim bunu ve alttakini
        List<Pokemon> GetDeletedPokemons();
        ICollection<Owner> GetOwnersOfAPokemon(int pokemonId);  //pokemonun sahibini getir dedik
        ICollection<Category> GetCategoriesOfPokemon(int pokemonId);
        //pokemonun kategorisiini getir dedik
        ICollection<Food> GetFoodsOfPokemon(int pokemonId);
        bool Save();
    }
}