using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;
using System.Security.Claims;


namespace PokemonReviewApp.Repository
{
    public class FoodRepository : IFoodRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FoodRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
         
            _httpContextAccessor = httpContextAccessor;
        }

        public Food GetFood(int id)
        {
            return _context.Foods
                .Include(f => f.FoodType)
                .FirstOrDefault(f => f.Id == id);
        }
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        public Food GetFoodByName(string name)
        {
            return _context.Foods
                .Include(f => f.FoodType)
                .FirstOrDefault(f => f.Name.Trim().ToUpper() == name.Trim().ToUpper());
        }

        public bool FoodExists(int foodId)
        {
            return _context.Foods.Any(f => f.Id == foodId);
        }

        public bool CreateFood(Food food)
        {
            food.CreatedUserId = GetUserId();
            food.CreatedUserDateTime = DateTime.Now;

            _context.Add(food);
            return Save();
        }

        public bool UpdateFood(Food food)
        {
            var existing = _context.Foods.FirstOrDefault(c => c.Id == food.Id);
            if (existing == null) return false;

            // GÜNCELLENECEK ALANLAR
            existing.Name = food.Name;
            existing.Price = food.Price;
            existing.FoodTypeId = food.FoodTypeId;

            // BU ALANLARI KORU
            existing.CreatedUserId = existing.CreatedUserId;
            existing.CreatedUserDateTime = existing.CreatedUserDateTime;

            _context.Update(existing);
            return Save();
        }


        public bool DeleteFood(int id)
        {
            var food = GetFood(id);
            if (food == null) return false;

            _context.Remove(food);
            return Save();
        }

        public ICollection<Food> GetAllFoods()
        {
            return _context.Foods
                .Include(f => f.FoodType)
                .OrderBy(f => f.Name)
                .ToList();
        }

        public ICollection<Food> GetFoodsByFoodType(int foodTypeId)
        {
            return _context.Foods
                .Where(f => f.FoodTypeId == foodTypeId)
                .Include(f => f.FoodType)
                .OrderBy(f => f.Name)
                .ToList();
        }

        public ICollection<Pokemon> GetPokemonsOfFood(int foodId)
        {
            var pokemons = (from pf in _context.PokemonFood
                            join p in _context.Pokemon
                                on pf.PokemonId equals p.Id
                            where pf.FoodId == foodId
                            select p)
                           .ToList();

            return pokemons;
        }

        // Yeni: Aktif (silinmemiş) Food listesi
        public ICollection<Food> GetActiveFoods()
        {
            return _context.Foods
                .Where(f => !f.IsDeleted) 
                .Include(f => f.FoodType)
                .OrderBy(f => f.Name)
                .ToList();
        }

        
        public bool SoftDeleteFood(int foodId)
        {
            var food = GetFood(foodId);
            if (food == null)
                return false;

            food.IsDeleted = true; 
            _context.Update(food);
            return Save();
        }


        public FoodType GetFoodTypeOfFood(int foodId)
        {
            var foodType = (from f in _context.Foods
                            join ft in _context.FoodTypes on f.FoodTypeId equals ft.Id
                            where f.Id == foodId
                            select ft).FirstOrDefault();

            return foodType;
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
