using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
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
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRepository(DataContext context, IUserRoleRepository userRoleRepository)
        {
            _context = context;
            _userRoleRepository = userRoleRepository;
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

        public async Task<bool> CreateUserWithLogAsync(User user, int roleId)
        {
            // Şifreyi hashle
            user.Password = HashHelper.ComputeSha512Hash(user.Password);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // User ekle
                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // user.Id atanır

                if (user.Id <= 0)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("User kaydedilemedi, Id alınamadı.");
                    return false;
                }

                // Rol ata
                var roleAssigned = await _userRoleRepository.CreateUserRoleAsync(new UserRoleDto
                {
                    UserId = user.Id,
                    RoleId = roleId
                });

                if (!roleAssigned)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Role atama başarısız.");
                    return false;
                }


                if (!roleAssigned)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Role atama başarısız.");
                    return false;
                }

                // UserLog oluştur
                var userLog = new UserLog
                {
                    UserId =9999999,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Password = user.Password,
                    CreatedDate = DateTime.Now
                };

                // Validasyon
                if (string.IsNullOrWhiteSpace(userLog.Name) ||
                    string.IsNullOrWhiteSpace(userLog.Email) ||
                    string.IsNullOrWhiteSpace(userLog.Password))
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("UserLog geçersiz veri içeriyor.");
                    return false;
                }

                _context.UserLog.Add(userLog);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Hata oluştu: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);

                await transaction.RollbackAsync();
                return false;
            }
        }


        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
