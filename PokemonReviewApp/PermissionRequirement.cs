using Microsoft.AspNetCore.Authorization;

namespace PokemonReviewApp
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public List<string> Permissions { get; }

        public PermissionRequirement(IEnumerable<string> permissions)
        {
            Permissions = permissions.Select(p => p.Trim()).ToList();
        }
    }

}