using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class FoodTypeRepository : IFoodTypeRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FoodTypeRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        
        public ICollection<FoodType> GetFoodTypes()//ssilinenleri göstermicek diğerlerini göstericek 
        {
            return _context.FoodTypes.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        }
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
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
            foodType.CreatedUserId = GetUserId();
            foodType.CreatedUserDateTime = DateTime.Now;

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
