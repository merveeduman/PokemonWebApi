using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;

namespace PokemonReviewApp.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        public ICollection<Role> GetRoles()
        {
            return _context.Roles
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Id)
                .ToList();
        }

        public Role GetRoleById(int id)
        {
            return _context.Roles
                .FirstOrDefault(r => r.Id == id && !r.IsDeleted);
        }

        public bool RoleExists(int id)
        {
            return _context.Roles.Any(r => r.Id == id && !r.IsDeleted);
        }

        public bool CreateRole(Role role)
        {
            role.CreatedUserId = GetUserId();
            role.CreatedUserDateTime = DateTime.UtcNow;
            role.IsDeleted = false;

            _context.Roles.Add(role);
            return Save();
        }

        public bool UpdateRole(Role updatedRole)
        {
            var existingRole = _context.Roles.FirstOrDefault(r => r.Id == updatedRole.Id && !r.IsDeleted);
            if (existingRole == null)
                return false;

            // Güncellenecek alanlar
            existingRole.Name = updatedRole.Name;
            existingRole.Description = updatedRole.Description;


            return Save();
        }

        public bool SoftDeleteRole(int roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId && !r.IsDeleted);
            if (role == null) return false;

            role.IsDeleted = true;

            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
