using System.ComponentModel.DataAnnotations;
using AutoMapper;
using PokemonReviewApp.Models;
using PokemonReviewApp.Dto;
namespace PokemonReviewApp.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public bool IsDeleted { get; set; } = false;   //  soft delete için ekledim
        public ICollection<Review> Reviews { get; set; }
        public ICollection<PokemonOwner> PokemonOwners { get; set; }
        public ICollection<PokemonCategory> PokemonCategories { get; set; }

        public ICollection<PokemonFood>PokemonFoods { get; set; } //food için many to many ilişkisi kurmak için kullanılır

        
    }

}