using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCarInfo.Data;
using MyCarInfo.Models;

namespace MyCarInfo.Services.Car
{
    public class CarService : ICarService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarService(
            IServiceScopeFactory scopeFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _scopeFactory = scopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetCarsCountAsync()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Cars.AsNoTracking().CountAsync();
        }

        public async Task<List<Vehicle>> GetAllCarsAsync()
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Cars
                .Include(c => c.Images)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Vehicle?> GetCarByIdAsync(int id)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Cars
                .Include(c => c.Images)
                .Include(c => c.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCarAsync(CarModel carModel)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Missing HTTP context.");
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(httpContext.User);

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
                    UserId = user.Id
                };

                user.Cars.Add(car);
                await context.Cars.AddAsync(car);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public async Task UpdateCarAsync(CarModel carModel)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var car = new Vehicle
            {

            };

            context.Cars.Update(car);
            await context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var car = await context.Cars.FindAsync(id);
            if (car != null)
            {
                context.Cars.Remove(car);
                await context.SaveChangesAsync();
            }
        }
    }
}
