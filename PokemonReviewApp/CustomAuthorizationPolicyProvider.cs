using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace PokemonReviewApp
{
    public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX = "Permission:";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetDefaultPolicyAsync() =>
            FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
            FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var permissionPart = policyName.Substring(POLICY_PREFIX.Length);
                var permissions = permissionPart.Split(',', StringSplitOptions.RemoveEmptyEntries);

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permissions));
                return Task.FromResult(policy.Build());
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }

}
