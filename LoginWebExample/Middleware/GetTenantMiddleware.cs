using System.Security.Claims;
using LoginWebExample.Helpers;
using LoginWebExample.ViewModel;

namespace LoginWebExample.Middleware
{
    public class GetTenantMiddleware
    {
        private readonly RequestDelegate _next;

        public GetTenantMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var payload = GetClaims(context.User.Claims);

                context.Items["Tenant"] = new Tenant()
                {
                    UserId = payload.ID,
                    Username = payload.Username,
                    UniqueId = payload.UniqueId
                };
            }

            await _next(context);
        }

        private static AuthenticationModel GetClaims(IEnumerable<Claim> claims)
        {
            return new()
            {
                ID = int.Parse(claims.FirstOrDefault(x => x.Type.Contains(AuthClaimType.UserId))!.Value),
                Username = claims.FirstOrDefault(x => x.Type.Contains(AuthClaimType.Username))!.Value,
                UniqueId = claims.FirstOrDefault(x => x.Type.Contains(AuthClaimType.UniqueId))!.Value
            };
        }
    }
}