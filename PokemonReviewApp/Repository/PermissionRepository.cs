using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PokemonReviewApp.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
      


        public PermissionRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public ICollection<Permission> GetPermissions()
        {
            return _context.Permission
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToList();
        }
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            return claim != null ? int.Parse(claim.Value) : 0;
        }

        public Permission GetPermissionById(int id)
        {
            return _context.Permission
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefault();
        }

        public bool PermissionExists(int id)
        {
            return _context.Permission.Any(p => p.Id == id && !p.IsDeleted);
        }

        public bool CreatePermission(Permission permission)
        {
            permission.CreatedUserId = GetUserId();
            permission.CreatedUserDateTime = DateTime.Now;

            _context.Add(permission);
            return Save();
        }

        public bool UpdatePermission(Permission permission)
        {
            var existingPermission = _context.Permission.FirstOrDefault(p => p.Id == permission.Id);

            if (existingPermission == null)
                return false;

            // Manuel güncelleme
            existingPermission.Name = permission.Name;
            existingPermission.Description = permission.Description;

            return Save();
        }


        public bool SoftDeletePermission(int permissionId)
        {
            var permission = _context.Permission.FirstOrDefault(p => p.Id == permissionId);
            if (permission == null) return false;

            permission.IsDeleted = true;
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
