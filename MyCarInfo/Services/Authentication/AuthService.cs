using Microsoft.AspNetCore.Identity;
using MyCarInfo.Data;
using MyCarInfo.Models;

namespace MyCarInfo.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result> RegisterAsync(RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
                return new Result { Succeeded = false, Error = "Passwords do not match." };

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
                return new Result { Succeeded = false, Error = error };
            }

            return new Result { Succeeded = true };
        }

        public async Task<Result> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return new Result { Succeeded = false, Error = "Invalid username or password." };

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
                return new Result { Succeeded = false, Error = "Invalid username or password." };

            return new Result { Succeeded = true };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
