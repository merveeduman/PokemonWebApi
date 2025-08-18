using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IRoleRepository
    {
        ICollection<Role> GetRoles();
        Role GetRoleById(int id);
        bool CreateRole(Role role);
        bool UpdateRole(Role role);
        bool SoftDeleteRole(int id);
        bool RoleExists(int id);
        bool Save();
    }

}
