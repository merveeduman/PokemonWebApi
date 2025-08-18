using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Controllers.Data;
using PokemonReviewApp.Hash;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokemonReviewApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Id)
                .ToList();
        }

        public User GetUserById(int id)
        {
            return _context.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefault();
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefault(u => u.Email == email);
        }




        public bool CreateUser(User user)
        {
            user.Password = HashHelper.ComputeSha512Hash(user.Password);
            _context.Users.Add(user);
            return Save();
        }

        public bool UpdateUser(User updatedUser)
        {

            return Save(); // SaveChanges çağırıyor
        }


        public bool SoftDeleteUser(int id)
        {
            var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == id);
            if (user == null || user.IsDeleted)
                return false;

            user.IsDeleted = true;
            _context.Users.Update(user);
            return Save();
        }

        public ICollection<Permission> GetUserPermissions(int userId)   //kullanıcı izinlerini almak için ekledim
        {
            var permissions = _context.UserRoles //UserRoles tablsouna erişiyoruz burada ve silinmemiş kayıtları alıyoruz
                .Where(ur => ur.UserId == userId && !ur.IsDeleted) 
                .Join(_context.RolePermission,// RolePermission tablosuyla  birleştirme yapıyoruz  (bir rolün hangi izinlere sahip olduğunu belirtir)
                      ur => ur.RoleId,  //UserRole'deki roleId ile
                      rp => rp.RoleId, //RolePermission'daki RoleId ile eşleşen kayıtları alıyoruz.
                      (ur, rp) => rp)  //sonuç olarak rolepermission neslerini alınır
                .Join(_context.Permission,  //permission tablosuyla birleştiriyoruz.
                      rp => rp.PermissionId,  //RolePermission.PermissionId ile 
                      p => p.Id,  //Permission.Id'deki  eşleşen kayıtlar alınır
                      (rp, p) => p)  //permission nesneleri elde edilir.
               
                .ToList();

            return permissions; // izinleri döndür
        }



        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
