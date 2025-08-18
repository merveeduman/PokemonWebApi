using PokemonReviewApp.Controllers.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokemonReviewApp.Repository
{
    public class FoodTypeRepository : IFoodTypeRepository
    {
        private readonly DataContext _context;

        public FoodTypeRepository(DataContext context)
        {
            _context = context;
        }

        
        public ICollection<FoodType> GetFoodTypes()//ssilinenleri göstermicek diğerlerini göstericek 
        {
            return _context.FoodTypes.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        }

        public FoodType GetFoodType(int foodTypeId)
        {
            return _context.FoodTypes.FirstOrDefault(ft => ft.Id == foodTypeId);
        }

        public bool FoodTypeExists(int foodTypeId)
        {
            return _context.FoodTypes.Any(ft => ft.Id == foodTypeId);
        }

        public bool CreateFoodType(FoodType foodType)
        {
            _context.Add(foodType);
            return Save();
        }
        public bool UpdateFoodType(FoodType foodType)
        {
            _context.Update(foodType);
            return Save();
        }



        public bool SoftDeleteFoodType(int foodTypeId)
        {
            var food = GetFoodType(foodTypeId);
            if (food == null)
                return false;

            food.IsDeleted = true;
            _context.Update(food);
            return Save();
        }




        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
