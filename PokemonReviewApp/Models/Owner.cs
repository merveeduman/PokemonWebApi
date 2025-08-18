using PokemonReviewApp.Dto;

namespace PokemonReviewApp.Models
{
    public class Owner
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gym { get; set; }
        public Country Country { get; set; }

        public bool IsDeleted { get; set; } = false;
        public ICollection<PokemonOwner> PokemonOwners { get; set; }

     
        public ICollection<PokemonDto> Pokemon { get; set; }
    }
}
