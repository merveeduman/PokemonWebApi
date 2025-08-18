using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Controllers.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokemonReviewApp.Repository
{
    public class FoodRepository : IFoodRepository
    {
        private readonly DataContext _context;

        public FoodRepository(DataContext context)
        {
            _context = context;
        }

        public Food GetFood(int id)
        {
            return _context.Foods
                .Include(f => f.FoodType)
                .FirstOrDefault(f => f.Id == id);
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
            _context.Add(food);
            return Save();
        }

        public bool UpdateFood(Food food)
        {
            _context.Update(food);
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
