using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryRepository(DataContext context, IHttpContextAccessor httpContextAccessor )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        public bool CreateCategory(Category category)
        {
            category.CreatedUserId = GetUserId();
            category.CreatedUserDateTime = DateTime.Now;

            _context.Add(category);
            return Save();
        }

        /*public bool DeleteCategory(Category category)
        {
            _context.Remove(category);
            return Save();
        }
        */
        public ICollection<Category> GetCategories()
        {
            return _context.Categories.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToList();
        }


        public Category GetCategory(int id)
        {
            return _context.Categories.Where(e => e.Id == id).FirstOrDefault();



        }

        public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
        {
            return _context.PokemonCategories.Where(e => e.CategoryId == categoryId).Select(c => c.Pokemon).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(CategoryDto dto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == dto.Id);
            if (category == null) return false;

            category.Name = dto.Name;

            return Save();
        }


        public bool SoftDeleteCategory(int id)
        {
            var category = GetCategory(id);
            if (category == null) return false;

            category.IsDeleted = true;
            _context.Update(category);

            return Save();
        }
        public BulkDeleteResultDto BulkDelete(List<int> ids)
        {
            var result = new BulkDeleteResultDto();

            // Silinecek kayıtları bul
            var categories = _context.Categories
                                     .Where(c => ids.Contains(c.Id))
                                     .ToList();

            var deletedIds = categories.Select(c => c.Id).ToList();
            var notFoundIds = ids.Except(deletedIds).ToList();

            // Gerçekten silmek yerine soft delete yapıyoruz
            foreach (var category in categories)
            {
                category.IsDeleted = true;
            }

            _context.SaveChanges();

            result.DeletedIds = deletedIds;
            result.NotFoundIds = notFoundIds;

            return result;
        }

    }
}