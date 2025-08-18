using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace PokemonReviewApp
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Kullanıcının "permission" claim'leri içinde gerekli izin var mı kontrolü
            bool hasPermission = context.User.Claims.Any(c =>
                c.Type == "permission" &&
                c.Value.Equals(requirement.Permission, System.StringComparison.OrdinalIgnoreCase));

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
