using System.ComponentModel.DataAnnotations;

namespace PokemonReviewApp.Dto
{
    public class UserDto
    {
       
            public int Id { get; set; }

            
            public string Name { get; set; }
            public string Surname { get; set; }
          
            public string Email { get; set; }
            public string Password { get; set; }
          
            


    }
}
