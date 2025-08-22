namespace PokemonReviewApp.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }  // Birincil anahtar (opsiyonel)
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public bool IsDeleted { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

    }
}
