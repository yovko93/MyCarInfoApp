namespace MyCarInfo.Services.Authentication
{
    using System.Threading.Tasks;
    using MyCarInfo.Models;

    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterModel model);
        Task<Result> LoginAsync(LoginModel model);
        Task LogoutAsync();
    }
}
