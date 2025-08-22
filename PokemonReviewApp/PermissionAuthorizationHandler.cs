using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace PokemonReviewApp
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userPermissions = context.User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            // OR mantığı — herhangi biri eşleşirse başarılı
            if (userPermissions.Any(userClaim => requirement.Permissions.Contains(userClaim)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}
