using System;
using System.ComponentModel.DataAnnotations;

namespace PokemonReviewApp.Models
{
    public class AuthLog
    {
        [Key]
        public int LoginId { get; set; }   // Primary Key
        public int UserId { get; set; }    
        public DateTime LoginDate { get; set; } = DateTime.Now;
        public User User { get; set; }
    }
}
