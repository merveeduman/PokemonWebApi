using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PokemonReviewApp.Dto
{
    public class RolePermissionDto
    {

            public int RoleId { get; set; }
            public string RoleName { get; set; }

            public int PermissionId { get; set; }
            public string PermissionName { get; set; }
        

    }
}
