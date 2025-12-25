using Microsoft.AspNetCore.Identity;
using MyCarInfo.Data;
using MyCarInfo.Models;

namespace MyCarInfo.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> RegisterAsync(RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                _logger.LogError("The password doesn't match!");
                return new Result { Succeeded = false, Error = "Паролата не съвпада." };
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                string error = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Registration failed for {Username}. Errors: {Errors}", model.Username, error);
                return new Result { Succeeded = false, Error = error };
            }

            _logger.LogInformation("User registered: {Username} ({Email})", user.UserName, user.Email);
            return new Result { Succeeded = true };
        }
    }
}
