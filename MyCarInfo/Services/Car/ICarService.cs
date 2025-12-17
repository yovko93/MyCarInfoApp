using MyCarInfo.Data;
using MyCarInfo.Models;

namespace MyCarInfo.Services.Car
{
    public interface ICarService
    {
        Task<int> GetCarsCountAsync();
        Task<List<Vehicle>> GetAllCarsAsync();
        Task<Vehicle?> GetCarByIdAsync(int id);
        Task<List<Vehicle>> GetCurrentUserCarsAsync();
        Task AddCarAsync(CarModel carModel);
        Task UpdateCarAsync(CarModel carModel);
        Task DeleteCarAsync(int id);
    }
}
