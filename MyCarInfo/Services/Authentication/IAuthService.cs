using MyCarInfo.Models;

namespace MyCarInfo.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterModel model);
    }
}
