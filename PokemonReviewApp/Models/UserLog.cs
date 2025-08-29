using System.ComponentModel.DataAnnotations;

namespace PokemonReviewApp.Models
{
    public class UserLog
    {
        [Key]
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
