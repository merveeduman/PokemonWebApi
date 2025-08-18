using Microsoft.AspNetCore.Authorization;

namespace PokemonReviewApp
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }


        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}