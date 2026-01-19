using MyCarInfo.Models;
using System.Security.Claims;

namespace MyCarInfo.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterModel model);
        Task<Result> UpdateProfileAsync(ProfileUpdateModel model, ClaimsPrincipal userPrincipal);
        Task<Result> ChangePasswordAsync(ChangePasswordModel model, ClaimsPrincipal userPrincipal);
    }
}
