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
                return new Result { Succeeded = false, Error = "Паролата не съвпада." };

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
    }
}
