using PokemonReviewApp.Controllers.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokemonReviewApp.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext _context;

        public RoleRepository(DataContext context)
        {
            _context = context;
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
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstOrDefault();
        }

        public bool RoleExists(int id)
        {
            return _context.Roles.Any(r => r.Id == id && !r.IsDeleted);
        }

        public bool CreateRole(Role role)
        {
            _context.Roles.Add(role);
            return Save();
        }

        public bool UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            return Save();
        }

        public bool SoftDeleteRole(int roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
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
