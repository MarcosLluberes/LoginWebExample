using System.Text;
using System.Globalization;
using System.Security.Claims;
using LoginWebExample.Options;
using LoginWebExample.ViewModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LoginWebExample.Helpers
{
    public static class JwtHelper
    {
        private static IEnumerable<Claim> GetClaims(AuthenticationModel model)
        {
            List<Claim> claims = new()
            {
                new Claim(AuthClaimType.UniqueId, model.UniqueId),
                new Claim(AuthClaimType.Username, model.Username),
                new Claim(AuthClaimType.UserId, model.ID.ToString()),
                new Claim(AuthClaimType.GuidId, model.GuidID ?? string.Empty),
                new Claim(AuthClaimType.ExpiredTime, model.ExpiredTime?.ToString("dd/MM/yyyy HH:mm:ss.fff")!)
            };

            return claims.ToArray();
        }

        private static SecurityToken GetTokenObject(string token, IConfiguration config)
        {
            JwtSettings settings = new();
            config.GetSection("Jwt").Bind(settings);

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Key)),
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return validatedToken;
        }

        public static string GenTokenKey(AuthenticationModel model, IConfiguration config, int? expiredTimeInminutes = 0)
        {
            try
            {
                if (model == null) return null!;

                JwtSettings settings = new();
                config.GetSection("Jwt").Bind(settings);

                // Get expire time
                expiredTimeInminutes = !expiredTimeInminutes.HasValue || expiredTimeInminutes == 0 ? int.Parse(settings.DefExpireTime ?? "120") : expiredTimeInminutes;
                DateTime expireTime = DateTime.Now.AddMinutes(expiredTimeInminutes.Value);
                model.ExpiredTime = expireTime;

                // Generate new guid string, help determine user login
                if (string.IsNullOrEmpty(model.GuidID)) model.GuidID = Guid.NewGuid().ToString();

                //Generate Token for user 
                var JWToken = new JwtSecurityToken(
                    issuer: settings.Issuer,
                    audience: settings.Audience,
                    claims: GetClaims(model),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(expireTime).DateTime,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Key)),
                        SecurityAlgorithms.HmacSha256
                    )
                );

                return new JwtSecurityTokenHandler().WriteToken(JWToken);
            }
            catch
            {
                return null!;
            }
        }

        public static AuthenticationModel ExtracToken(string token, IConfiguration config)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return null!;

                var jwtToken = (JwtSecurityToken)GetTokenObject(token, config);
                var claims = jwtToken.Claims;

                return new()
                {
                    ID = int.Parse(claims.First(x => x.Type.Contains(AuthClaimType.UserId)).Value),
                    UniqueId = claims.First(x => x.Type.Contains(AuthClaimType.UniqueId)).Value,
                    Username = claims.First(x => x.Type.Contains(AuthClaimType.Username)).Value,
                    GuidID = claims.First(x => x.Type.Contains(AuthClaimType.GuidId)).Value,
                    ExpiredTime = (claims.First(x => x.Type.Contains(AuthClaimType.ExpiredTime)).Value).AsDateTimeExac(DateTime.Now)
                };
            }
            catch
            {
                return null!;
            }
        }

        public static DateTime AsDateTimeExac(this object obj, DateTime defaultValue = default)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString())) return defaultValue;

            if (!DateTime.TryParseExact(obj.ToString(), "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result)) return defaultValue;

            return result;
        }

        public static bool IsTokenValid(string token, IConfiguration config)
        {
            if (string.IsNullOrEmpty(token)) return false;
            var payload = ExtracToken(token, config);

            return payload != null;
        }

        public static Tenant GetTenant(this HttpContext context) => (Tenant)context.Items["Tenant"]!;
    }
}