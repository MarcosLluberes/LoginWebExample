using LoginWebExample.Helpers;
using LoginWebExample.Options;
using Microsoft.AspNetCore.Mvc;
using LoginWebExample.ViewModel;
using LoginWebExample.ViewResult;
using LoginWebExample.ExampleModel;

namespace LoginWebExample.Controllers
{
    [ApiController, Route("auth/[action]"), ProducesResponseType(200, Type = typeof(AuthResult))]
    public class AuthController : ControllerBase
    {
        readonly AppSettings _settings;
        readonly IConfiguration _config;
        readonly ExampleDbContext _context;

        public AuthController(ExampleDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;

            _settings = new();
            config.GetSection("AppSettings").Bind(_settings);
        }

        // Log in user
        [HttpPost]
        public IActionResult UserLogin(LoginModel model)
        {
            string errorCode = "USER_NOT_EXIST";

            // Look for user
            var access = _context.Users
                .FirstOrDefault(x => x.Usuario == model.UserName);

            if (access != null)
            {
                errorCode = "USER_SUSPENDED";

                if (!access.Suspended)
                {
                    errorCode = "LOGIN_ATTEMPT_EXCEEDED";

                    var lastAttempAt = 0d;
                    byte loginAttempts = access.LoginFailAttempts ?? 0;

                    if (access.LastLoginFail != null)
                        lastAttempAt = (DateTime.Now - (DateTime)access.LastLoginFail).TotalMinutes;

                    if ((access.LoginFailAttempts == null && access.LastLoginFail == null) || access.LoginFailAttempts < _settings.MaxLoginAttempt || lastAttempAt > _settings.ResetLoginAttemptAt)
                    {
                        errorCode = "WRONG_PASSWORD";
                        var user = _context.Users.First(x => x.Id == access.Id);

                        if (SecurePassword.ValidatePassword(model.Password, access.Passcode))
                        {
                            // Set the auth model
                            var authModel = new AuthenticationModel()
                            {
                                ID = access.Id,
                                UniqueId = access.TenandId,
                                Username = access.Usuario
                            };

                            // Get token
                            string token = JwtHelper.GenTokenKey(authModel, _config);
                            DateTime expireDate = (DateTime)authModel.ExpiredTime!;

                            // Update user
                            user.LastLogin = DateTime.Now;
                            user.LastLoginFail = null;
                            user.LoginFailAttempts = null;

                            _context.SaveChanges();

                            return Ok(new AuthResult
                            {
                                ID = access.Id,
                                Username = user.Usuario,
                                CompleteName = $"{user.Nombre} {user.Apellido}".Trim(),
                                Token = token,
                                Expires = expireDate
                            });
                        }

                        loginAttempts++;
                        user.LoginFailAttempts = loginAttempts;
                        user.LastLoginFail = DateTime.Now;

                        _context.SaveChanges();
                    }
                }
            }

            return Unauthorized(errorCode);
        }

        // Verify token validity
        [HttpPost, ProducesResponseType(200)]
        public IActionResult VerifyToken(TokenModel model)
        {
            if (string.IsNullOrEmpty(model.Token)) return Unauthorized("TOKEN_NOT_FOUND");
            if (!JwtHelper.IsTokenValid(model.Token, _config)) return Unauthorized("TOKEN_INVALID");

            return Ok();
        }

        // Refresh token
        [HttpPost]
        public IActionResult RefreshToken(TokenModel model)
        {
            if (string.IsNullOrEmpty(model.Token)) return Unauthorized("TOKEN_NOT_FOUND");

            if (!JwtHelper.IsTokenValid(model.Token, _config)) return Unauthorized("TOKEN_INVALID");

            // Get token payload
            var payload = JwtHelper.ExtracToken(model.Token, _config);

            payload.GuidID = null!;
            payload.ExpiredTime = null;

            // Get new token
            string newToken = JwtHelper.GenTokenKey(payload, _config);
            DateTime expireDate = (DateTime)payload.ExpiredTime!;

            // Complete auth result information
            var user = _context.Users
                .First(x => x.Id == payload.ID);

            return Ok(new AuthResult
            {
                ID = user.Id,
                Username = user.Usuario,
                CompleteName = $"{user.Nombre} {user.Apellido}".Trim(),
                Token = newToken,
                Expires = expireDate
            });
        }
    }
}