using PokemonReviewApp.Models;
using System.Collections.Generic;

namespace PokemonReviewApp.Interfaces
{
    public interface IUserRepository
    {
        ICollection<Permission> GetUserPermissions(int userId); //kullanıcı izinlerini getirmek için

        IEnumerable<User> GetUsers();
        User GetUserByEmail(string email);
        User GetUserById(int id);
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool SoftDeleteUser(int id);
        Task<bool> CreateUserWithLogAsync(User user, int roleId);

        bool Save();
    }
}
