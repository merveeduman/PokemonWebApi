using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Controllers.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokemonReviewApp.Repository
{
    public class PokemonFoodRepository : IPokemonFoodRepository
    {
        private readonly DataContext _context;

        public PokemonFoodRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<PokemonFood> GetPokemonFoods()
        {
            var query = from pf in _context.PokemonFood //POKEMON FOOD TABLOSUNDAKI TÜM KAYITLAR ALINIR
                        join f in _context.Foods on pf.FoodId equals f.Id //PokemonFood ile Food tablosu FoodId üzerinden join edilir
                        join p in _context.Pokemon on pf.PokemonId equals p.Id //PokemonFood ile Pokemon tablosu da PokemonId üzerinden join edilir.
                        where !pf.IsDeleted //isDeleted false olan kayıtlar silinir
                        select new PokemonFood  //her eşleşmeden yeni bir PkemonFood nesnesi oluşturuluyor
                        {
                            PokemonId = pf.PokemonId,
                            FoodId = pf.FoodId,
                            Food = f,
                            Pokemon = p
                        };

            return query.ToList();   //sorgu çalıştırılır ve sonuçlar liste olarak geri döndürülür.
        }


        public PokemonFood GetPokemonFood(int pokemonId, int foodId)
        {
            return _context.PokemonFood
                .Where(pf => pf.PokemonId == pokemonId && pf.FoodId == foodId)
                .Select(pf => new PokemonFood
                {
                    PokemonId = pf.PokemonId,
                    FoodId = pf.FoodId,
                    Food = pf.Food // EF Core burada ilişkili Food'u da getirecektir
                })
                .FirstOrDefault();
        }


        public bool PokemonFoodExists(int pokemonId, int foodId)
        {
            return _context.PokemonFood.Any(pf => pf.PokemonId == pokemonId && pf.FoodId == foodId);
        }

        public bool CreatePokemonFood(PokemonFood pokemonFood)
        {
            // FK kontrolleri
            bool pokemonExists = _context.Pokemon.Any(p => p.Id == pokemonFood.PokemonId);
            bool foodExists = _context.Foods.Any(f => f.Id == pokemonFood.FoodId);

            if (!pokemonExists || !foodExists)
            {
                return false; // Controller'da 404 döneceğiz
            }

            // Önce soft delete ile işaretlenmiş kaydı bulmaya çalış
            var existingSoftDeleted = _context.PokemonFood
                .FirstOrDefault(x => x.PokemonId == pokemonFood.PokemonId &&
                                     x.FoodId == pokemonFood.FoodId &&
                                     x.IsDeleted == true);

            if (existingSoftDeleted != null)
            {
                // Eğer soft delete edilmiş kayıt varsa, onu geri döndür
                existingSoftDeleted.IsDeleted = false;
                _context.PokemonFood.Update(existingSoftDeleted);
                _context.SaveChanges();
                return true;
            }

            // Soft delete edilmemiş kayıt var mı kontrol et (yine aynı kayıt)
            bool exists = _context.PokemonFood
                .Any(x => x.PokemonId == pokemonFood.PokemonId &&
                          x.FoodId == pokemonFood.FoodId &&
                          x.IsDeleted == false);

            if (exists)
            {
                return false; // Controller'da 409 Conflict döneceğiz
            }

            // Yeni kayıt ekle
            _context.Add(pokemonFood);
            return _context.SaveChanges() > 0;
        }

        public bool PokemonExists(int pokemonId)
        {
            return _context.Pokemon.Any(p => p.Id == pokemonId);
        }

        public bool FoodExists(int foodId)
        {
            return _context.Foods.Any(f => f.Id == foodId);
        }



        public bool UpdatePokemonFood(PokemonFood pokemonFood)
        {
            _context.Update(pokemonFood);
            return Save();
        }

        public bool DeletePokemonFood(PokemonFood pokemonFood)
        {
            _context.Remove(pokemonFood);
            return Save();
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
        public bool SoftDeletePokemonFood(int pokemonId, int foodId)
        {
            var pokemon = GetPokemonFood(pokemonId, foodId);
            if (pokemon == null) return false;

            pokemon.IsDeleted = true;
            _context.Update(pokemon);

            return Save();
        }

    }
}
