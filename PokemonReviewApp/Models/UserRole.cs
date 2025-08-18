namespace PokemonReviewApp.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public bool IsDeleted { get; set; }

        // Navigasyon propertileri
        public User User { get; set; }   // Foreign Key bağlantısı
        public Role Role { get; set; }   // Foreign Key bağlantısıv
    }

}
