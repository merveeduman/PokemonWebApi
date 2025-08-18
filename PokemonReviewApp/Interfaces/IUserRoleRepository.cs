using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{

    public interface IUserRoleRepository
    {
        bool AssignRoleToUser(int userId, int roleId);


        bool RemoveRoleFromUser(int userId, int roleId);
        ICollection<Role> GetRolesByUser(int userId);
        ICollection<User> GetUsersByRole(int roleId);
        ICollection<UserRoleDto> GetAllUserRolesWithDetails();

        ICollection<UserRole> GetAllUserRoles();
        bool SoftDeleteUserRole(int userId, int roleId);
        bool Save();
    }

}

