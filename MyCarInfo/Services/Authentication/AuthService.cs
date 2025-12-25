using Microsoft.AspNetCore.Identity;
using MyCarInfo.Data;
using MyCarInfo.Models;
using System.Security.Claims;

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

        public async Task<Result> UpdateProfileAsync(ProfileUpdateModel model, ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null)
            {
                _logger.LogWarning("Profile update failed. User not found.");
                return new Result { Succeeded = false, Error = "Не успяхме да заредим профила. Моля, опитайте отново." };
            }

            var emailResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!emailResult.Succeeded)
            {
                var emailError = string.Join(", ", emailResult.Errors.Select(error => error.Description));
                _logger.LogWarning("Profile update failed for {Username}. Email errors: {Errors}", user.UserName, emailError);
                return new Result { Succeeded = false, Error = emailError };
            }

            var phoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!phoneResult.Succeeded)
            {
                var phoneError = string.Join(", ", phoneResult.Errors.Select(error => error.Description));
                _logger.LogWarning("Profile update failed for {Username}. Phone errors: {Errors}", user.UserName, phoneError);
                return new Result { Succeeded = false, Error = phoneError };
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var updateError = string.Join(", ", updateResult.Errors.Select(error => error.Description));
                _logger.LogWarning("Profile update failed for {Username}. Update errors: {Errors}", user.UserName, updateError);
                return new Result { Succeeded = false, Error = updateError };
            }

            var wantsPasswordUpdate = !string.IsNullOrWhiteSpace(model.CurrentPassword)
                || !string.IsNullOrWhiteSpace(model.NewPassword)
                || !string.IsNullOrWhiteSpace(model.ConfirmNewPassword);

            if (wantsPasswordUpdate)
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword)
                    || string.IsNullOrWhiteSpace(model.NewPassword)
                    || string.IsNullOrWhiteSpace(model.ConfirmNewPassword))
                {
                    return new Result { Succeeded = false, Error = "За смяна на паролата попълни всички полета за парола." };
                }

                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    return new Result { Succeeded = false, Error = "Новата парола не съвпада." };
                }

                var passwordResult = await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    var passwordError = string.Join(", ", passwordResult.Errors.Select(error => error.Description));
                    _logger.LogWarning("Profile update failed for {Username}. Password errors: {Errors}", user.UserName, passwordError);
                    return new Result { Succeeded = false, Error = passwordError };
                }
            }

            _logger.LogInformation("Profile updated for {Username}.", user.UserName);
            return new Result { Succeeded = true };
        }
    }
}
