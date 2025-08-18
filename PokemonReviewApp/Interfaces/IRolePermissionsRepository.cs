using PokemonReviewApp.Models;
using System.Collections.Generic;

namespace PokemonReviewApp.Interfaces
{
    public interface IRolePermissionsRepository
    {
        ICollection<RolePermission> GetRolePermissions();
        RolePermission GetRolePermissionByIds(int roleId, int permissionId);
        bool CreateRolePermission(RolePermission rolePermission);
        bool RolePermissionExists(int roleId, int permissionId);
        bool SoftDeleteRolePermission(int roleId, int permissionId);

        bool Save();
    }
}
