using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
       ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Pokemon> GetPokemonByCategory(int categoryId);
        bool CategoryExists(int id);
        bool CreateCategory(Category category);
        bool UpdateCategory(CategoryDto dto);
        //bool DeleteCategory(Category category);


        BulkDeleteResultDto BulkDelete(List<int> ids);
        bool SoftDeleteCategory(int id);  // soft delete için yapıyyorum



       

        bool Save();
    }
}