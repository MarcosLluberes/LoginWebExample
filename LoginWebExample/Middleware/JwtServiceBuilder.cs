using System.Text;
using LoginWebExample.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LoginWebExample.Middleware
{
    public static class JwtServiceBuilder
    {
        public static AuthenticationBuilder AddJwtAuth(this IServiceCollection services, IConfiguration config)
        {
            JwtSettings settings = new();
            config.GetSection("Jwt").Bind(settings);

            return services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static IApplicationBuilder UseCustomAuth(this IApplicationBuilder app)
        {
            return app
                .UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<GetTenantMiddleware>();
        }
    }
}