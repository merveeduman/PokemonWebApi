using PokemonReviewApp.Controllers.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokemonReviewApp.Repository
{
    public class RolePermissionRepository : IRolePermissionsRepository
    {
        private readonly DataContext _context;

        public RolePermissionRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<RolePermission> GetRolePermissions()
        {
            var query = from rp in _context.RolePermission
                        join r in _context.Roles on rp.RoleId equals r.Id
                        join p in _context.Permission on rp.PermissionId equals p.Id
                        where !rp.IsDeleted
                        select new RolePermission
                        {
                            RoleId = rp.RoleId,
                            PermissionId = rp.PermissionId,
                            IsDeleted = rp.IsDeleted,
                            Role = r,
                            Permission = p
                        };

            return query.ToList();
        }


        public RolePermission GetRolePermissionByIds(int roleId, int permissionId)
        {
            return (from rp in _context.RolePermission
                    join r in _context.Roles on rp.RoleId equals r.Id
                    join p in _context.Permission on rp.PermissionId equals p.Id
                    where rp.RoleId == roleId && rp.PermissionId == permissionId && !rp.IsDeleted
                    select new RolePermission
                    {
                        RoleId = rp.RoleId,
                        PermissionId = rp.PermissionId,
                        IsDeleted = rp.IsDeleted,
                        Role = r,
                        Permission = p
                    }).FirstOrDefault();
        }

        public bool RolePermissionExists(int roleId, int permissionId)
        {
            return _context.RolePermission.Any(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }

        public bool CreateRolePermission(RolePermission rolePermission)
        {
            _context.RolePermission.Add(rolePermission);
            return Save();
        }

        public bool SoftDeleteRolePermission(int roleId, int permissionId)
        {
            if (roleId <= 0 || permissionId <= 0)
                return false;

            var rolePermission = GetRolePermissionByIds(roleId, permissionId);

            if (rolePermission == null)
                return false;

            rolePermission.IsDeleted = true;
            _context.RolePermission.Update(rolePermission);

            return Save();
        }


        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
