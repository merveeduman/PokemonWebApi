using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPermissionRepository
    {
        ICollection<Permission> GetPermissions();
        Permission GetPermissionById(int id);
        bool CreatePermission(Permission permission);
        bool UpdatePermission(Permission permission);
        bool SoftDeletePermission(int id);
        bool PermissionExists(int id);
        bool Save();
    }
}
