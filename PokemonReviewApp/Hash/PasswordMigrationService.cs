using PokemonReviewApp.Controllers.Data;

namespace PokemonReviewApp.Hash
{
    public class PasswordMigrationService
    {
        private readonly DataContext _context;

        public PasswordMigrationService(DataContext context)
        {
            _context = context;
        }

        public void MigratePasswords()
        {
            var users = _context.Users.Where(u => !u.IsDeleted).ToList(); //veri tabanından silinmemeiş tüm userları alıyoruz

            foreach (var user in users)
            {
                // hash'lenmemiş gibi görünen şifreler
           
                    user.Password = HashHelper.ComputeSha512Hash(user.Password); //burada düz şifretyi alıp metoda gönderiyoruz oradan dönen hash değerini yüklyoruz
                    _context.Users.Update(user);
                
            }

            _context.SaveChanges();
        }
    }

}
