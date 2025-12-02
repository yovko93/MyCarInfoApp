using MyCarInfo.Data;

namespace MyCarInfo.Services.Car
{
    public interface ICarService
    {
        Task<int> GetCarsCountAsync();
        Task<List<Vehicle>> GetAllCarsAsync();
        Task<Vehicle?> GetCarByIdAsync(int id);
        Task AddCarAsync(Vehicle car);
        Task UpdateCarAsync(Vehicle car);
        Task DeleteCarAsync(int id);
    }
}
