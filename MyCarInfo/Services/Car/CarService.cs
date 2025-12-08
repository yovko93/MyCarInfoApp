using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCarInfo.Data;
using MyCarInfo.Models;

namespace MyCarInfo.Services.Car
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<int> GetCarsCountAsync()
        {
            return await _context.Cars.CountAsync();
        }

        public async Task<List<Vehicle>> GetAllCarsAsync()
        {
            return await _context.Cars
                .Include(c => c.Images)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Vehicle?> GetCarByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Images)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCarAsync(CarModel carModel)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Missing HTTP context.");
                var user = await _userManager.GetUserAsync(httpContext.User);

                if (user == null)
                {
                    throw new InvalidOperationException("User must be logged in to add a car.");
                }

                var car = new Vehicle
                {
                    Model = carModel.Model,
                    Brand = carModel.Brand,
                    LicensePlate = carModel.LicensePlate,
                    Engine = carModel.Engine,
                    HorsePower = carModel.HorsePower,
                    Color = carModel.Color,
                    InspectionExpiryDate = carModel.InspectionExpiryDate,
                    InsuranceExpiryDate = carModel.InsuranceExpiryDate,
                    VignetteExpiryDate = carModel.VignetteExpiryDate,
                    Images = carModel.ImagePaths.Select(path => new CarImage
                    {
                        ImagePath = path
                    }).ToList(),
                    UserId = user.Id,
                    User = user
                };

                user.Cars.Add(car);
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
