using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyCarInfo.Data;
using System.ComponentModel.DataAnnotations;

namespace MyCarInfo.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string? InfoMessage { get; set; }

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Въведете потребител!")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Въведете парола!")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet(string? message = null, string? returnUrl = null)
        {
            ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
               ? Url.Content("~/")
               : returnUrl;

            InfoMessage = string.IsNullOrWhiteSpace(message) ? null : message;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null, string? message = null)
        {
            ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                ? Url.Content("~/")
                : returnUrl;

            InfoMessage = string.IsNullOrWhiteSpace(message) ? null : message;

            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Input.Username, Input.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in: {Username}", Input.Username);
                return LocalRedirect(ReturnUrl);
            }

            _logger.LogWarning("Failed login attempt for {Username}", Input.Username);
            ErrorMessage = "Невалиден потребител или грешна парола!";
            return Page();
        }
    }
}
