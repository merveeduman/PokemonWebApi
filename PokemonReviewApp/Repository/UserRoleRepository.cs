using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Security.Claims;


namespace PokemonReviewApp.Repository
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRoleRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool AssignRoleToUser(int userId, int roleId)
        {
            var exists = _context.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);
            if (exists) return false;

            var user = _context.Users.FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId && !r.IsDeleted);

            if (user == null || role == null)
                return false;

            _context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreatedUserId= userId,
                CreatedUserDateTime = DateTime.UtcNow,
            });

            return Save();
        }

        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
        public async Task<bool> CreateUserRoleAsync(UserRoleDto dto)
        {
            var userRole = new UserRole
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                IsDeleted = false,
                CreatedUserDateTime = DateTime.Now,
                CreatedUserId = GetUserId()
            };
            _context.UserRoles.Add(userRole);
            return await _context.SaveChangesAsync() > 0;
        }




        public ICollection<UserRole> GetAllUserRoles()
        {
            var query = from ur in _context.UserRoles
                        join u in _context.Users on ur.UserId equals u.Id
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where !ur.IsDeleted && !u.IsDeleted && !r.IsDeleted
                        select new UserRole
                        {
                            UserId = ur.UserId,
                            RoleId = ur.RoleId,
                            IsDeleted = ur.IsDeleted,
                            User = u,
                            Role = r
                        };

            return query.ToList();
        }

        public bool RemoveRoleFromUser(int userId, int roleId)
        {
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);
            if (userRole == null)
                return false;

            _context.UserRoles.Remove(userRole);
            return Save();
        }

        public ICollection<Role> GetRolesByUser(int userId)
        {
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == userId && !ur.IsDeleted && !r.IsDeleted
                        select r;

            return roles.ToList();
        }


        public ICollection<User> GetUsersByRole(int roleId)
        {
            var users = from ur in _context.UserRoles
                        join u in _context.Users on ur.UserId equals u.Id
                        where ur.RoleId == roleId && !ur.IsDeleted && !u.IsDeleted
                        select u;

            return users.ToList();
        }
        public ICollection<UserRoleDto> GetAllUserRolesWithDetails()
        {
            var query = from ur in _context.UserRoles
                        join u in _context.Users on ur.UserId equals u.Id
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where !ur.IsDeleted && !u.IsDeleted && !r.IsDeleted
                        select new UserRoleDto
                        {
                            UserId = ur.UserId,
                            RoleId = ur.RoleId,
                            IsDeleted = ur.IsDeleted,
                            RoleName = r.Name,
                            UserName = u.Name + u.Name
                        };

            return query.ToList();
        }


        public bool SoftDeleteUserRole(int userId, int roleId)
        {
            var userRole = _context.UserRoles

                .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted);

            if (userRole == null)
                return false;

            userRole.IsDeleted = true;
            _context.UserRoles.Update(userRole);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}