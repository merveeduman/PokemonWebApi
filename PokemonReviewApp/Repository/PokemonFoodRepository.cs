using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class PokemonFoodRepository : IPokemonFoodRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PokemonFoodRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
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

            // Önce soft delete ile işaretlenmiş kaydı bul
            var existingSoftDeleted = _context.PokemonFood
                .FirstOrDefault(x => x.PokemonId == pokemonFood.PokemonId &&
                                     x.FoodId == pokemonFood.FoodId &&
                                     x.IsDeleted == true);

            if (existingSoftDeleted != null)
            {
                // Soft delete edilmiş kaydı geri getir
                existingSoftDeleted.IsDeleted = false;
                existingSoftDeleted.CreatedUserId = GetUserId(); // 🔥 KULLANICI EKLENDİ
                existingSoftDeleted.CreatedUserDateTime = DateTime.Now; // 🔥 ZAMAN EKLENDİ

                _context.PokemonFood.Update(existingSoftDeleted);
                return Save(); // Save() metodunu kullan
            }

            // Zaten var olan aktif kayıt varsa, ekleme
            bool exists = _context.PokemonFood
                .Any(x => x.PokemonId == pokemonFood.PokemonId &&
                          x.FoodId == pokemonFood.FoodId &&
                          x.IsDeleted == false);

            if (exists)
            {
                return false; // 409 Conflict
            }

            // Yeni kayıt oluştur
            pokemonFood.CreatedUserId = GetUserId();           
            pokemonFood.CreatedUserDateTime = DateTime.Now;    

            _context.Add(pokemonFood);
            return Save(); // Save() metodunu kullan
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
