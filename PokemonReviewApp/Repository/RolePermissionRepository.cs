using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class RolePermissionRepository : IRolePermissionsRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RolePermissionRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
        public bool RolePermissionExists(int roleId, int permissionId)
        {
            return _context.RolePermission.Any(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }

        public bool CreateRolePermission(RolePermission rolePermission)
        {
            rolePermission.CreatedUserId = GetUserId();
            rolePermission.CreatedUserDateTime = DateTime.Now;

            _context.Add(rolePermission);
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
