using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCarInfo.Data;
using MyCarInfo.Models;
using System.Security.Claims;

namespace MyCarInfo.Services.Car
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCarsCountAsync()
        {
            return await _context.Cars.CountAsync();
        }

        public async Task<List<Vehicle>> GetAllCarsAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Vehicle?> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task AddCarAsync(CarModel carModel)
        {
            try
            {
                //var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

                var car = new Vehicle
                {
                    Model = carModel.Model,
                    Brand = carModel.Brand,
                    LicensePlate = carModel.LicensePlate,
                    Engine = carModel.Engine,
                    HorsePower = carModel.HorsePower,
                    InspectionExpiryDate = carModel.InspectionExpiryDate,
                    InsuranceExpiryDate = carModel.InsuranceExpiryDate,
                    VignetteExpiryDate = carModel.VignetteExpiryDate,
                    //UserId = userId,
                };

                await _context.Cars.AddAsync(car);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public async Task UpdateCarAsync(CarModel carModel)
        {
            var car = new Vehicle
            {

            };

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }
    }
}
