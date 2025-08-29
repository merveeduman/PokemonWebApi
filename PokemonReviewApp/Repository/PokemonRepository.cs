using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PokemonRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
        public bool CreatePokemon(int ownerId, int categoryId, int foodId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.FirstOrDefault(o => o.Id == ownerId);
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            var food = _context.Foods.FirstOrDefault(f => f.Id == foodId);

            if (pokemonOwnerEntity == null || category == null || food == null)
                return false;


            pokemon.CreatedUserId = GetUserId();
            pokemon.CreatedUserDateTime = DateTime.Now;

            // Pokemon-Owner ilişkisi
            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
                CreatedUserId = GetUserId(),
                CreatedUserDateTime = DateTime.Now
            };
            _context.Add(pokemonOwner);

            // Pokemon-Category ilişkisi
            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon,
                CreatedUserId = GetUserId(),
                CreatedUserDateTime = DateTime.Now
            };
            _context.Add(pokemonCategory);

            // Pokemon-Food ilişkisi
            var pokemonFood = new PokemonFood
            {
                Pokemon = pokemon,
                Food = food,
                CreatedUserId = GetUserId(),                       // Kullanıcı ID'sini al
                CreatedUserDateTime = DateTime.UtcNow              // Şu anki zamanı ver
            };
            _context.Add(pokemonFood);


            // Pokemon'u ekle
            _context.Add(pokemon);

            return Save();
        }



       /* public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }
        */
        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var review = _context.Reviews.Where(p => p.Pokemon.Id == pokeId);

            if (review.Count() <= 0)
                return 0;

            return ((decimal)review.Sum(r => r.Rating) / review.Count());
        }

        /*public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemon.OrderBy(p => p.Id).ToList();
        }
        */


        public ICollection<Owner> GetOwnersOfAPokemon(int pokemonId)
        {
            var owners = (from po in _context.PokemonOwners   //ara tablodan başlıyoruz.
                          join o in _context.Owners
                              on po.OwnerId equals o.Id  //Owner tablosuna bağlandık.

                          where po.PokemonId == pokemonId  //sadece istediğin Pokémon için filtre.
                          select o)   //sadece Owner listesi döndürülür.
                         .ToList();

            return owners;
        }


        public List<Pokemon> GetActivePokemons() //aktif pokemonaların gözükmesini sağlamak için kullanılıyor
        {
            return _context.Pokemon.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        }

        public List<Pokemon> GetDeletedPokemons()
        {
            return _context.Pokemon.Where(p => p.IsDeleted).OrderBy(p => p.Id).ToList();
        }

        public Pokemon GetPokemonTrimToUpper(PokemonDto pokemonCreate)
        {
            return _context.Pokemon
                .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.Trim().ToUpper())
                .FirstOrDefault();
        }


        public ICollection<Category> GetCategoriesOfPokemon(int pokemonId)
        {
            var category = (from pc in _context.PokemonCategories   //ara tablodan başlıyoruz.
                            join c in _context.Categories
                                on pc.CategoryId equals c.Id  //category tablosuna bağlandık.

                            where pc.PokemonId == pokemonId  //sadece istediğin Pokémon için filtre.
                            select c)   //sadece Owner listesi döndürülür.
                         .ToList();

            return category;
        }
        public ICollection<Food> GetFoodsOfPokemon(int pokemonId)
        {
            var foods = (from pf in _context.PokemonFood
                         join f in _context.Foods
                         on pf.FoodId equals f.Id
                         where pf.PokemonId == pokemonId
                         select f)
                        .ToList();

            return foods;
        }


        public bool PokemonExists(int pokeId)
        {
            return _context.Pokemon.Any(p => p.Id == pokeId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var existing = _context.Pokemon.FirstOrDefault(p => p.Id == pokemon.Id);
            if (existing == null) return false;

            // Güncellenecek alanlar
            existing.Name = pokemon.Name;
            existing.BirthDate = pokemon.BirthDate;

         

            // Oluşturulma bilgileri olduğu gibi kalacak
            existing.CreatedUserId = existing.CreatedUserId;
            existing.CreatedUserDateTime = existing.CreatedUserDateTime;

            _context.Update(existing);
            return Save();
        }
        public bool SoftDeletePokemon(int id)
        {
            var pokemon = _context.Pokemon.FirstOrDefault(p => p.Id == id);
            if (pokemon == null) return false;

            pokemon.IsDeleted = true;  
            _context.Update(pokemon);

            return Save();
        }





    }
}