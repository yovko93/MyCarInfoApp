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
        private readonly ILogger<CarService> _logger;

        public CarService(
            IServiceScopeFactory scopeFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CarService> logger)
        {
            _scopeFactory = scopeFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<int> GetCarsCountAsync()
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                return await context.Cars
                   .AsNoTracking()
                   .CountAsync(c => !c.IsDeleted);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load cars count. Returning 0.");
                return 0;
            }
        }

        public async Task<List<Vehicle>> GetAllCarsAsync()
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                return await context.Cars
                    .Where(c => !c.IsDeleted)
                    .Include(c => c.Images)
                    .Include(c => c.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load all cars. Returning empty list.");
                return new List<Vehicle>();
            }
        }

        public async Task<List<Vehicle>> GetCurrentUserCarsAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return new List<Vehicle>();
            }

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(httpContext.User);

                if (user is null)
                {
                    return new List<Vehicle>();
                }

                return await context.Cars
                     .Where(c => c.UserId == user.Id && !c.IsDeleted)
                     .Include(c => c.Images)
                     .Include(c => c.User)
                     .AsNoTracking()
                     .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load current user cars. Returning empty list.");
                return new List<Vehicle>();
            }
        }

        public async Task<Vehicle?> GetCarByIdAsync(int id)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                return await context.Cars
                    .Where(c => !c.IsDeleted)
                    .Include(c => c.Images)
                    .Include(c => c.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load car by id {CarId}. Returning null.", id);
                return null;
            }
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

                _logger.LogInformation("Car registered for user {UserName}: {Brand} {Model} ({LicensePlate})", user.UserName, car.Brand, car.Model, car.LicensePlate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to register car.");
            }
        }

        public async Task UpdateCarAsync(CarModel carModel)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Missing HTTP context.");
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(httpContext.User) ?? throw new InvalidOperationException("User must be logged in to update a car.");

                var car = await context.Cars
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == carModel.Id);

                if (car is null)
                {
                    throw new InvalidOperationException($"Car with id {carModel.Id} was not found.");
                }

                if (car.UserId != user.Id)
                {
                    throw new UnauthorizedAccessException("You are not authorized to update this car.");
                }

                car.Brand = carModel.Brand;
                car.Model = carModel.Model;
                car.LicensePlate = carModel.LicensePlate;
                car.Engine = carModel.Engine;
                car.HorsePower = carModel.HorsePower;
                car.Color = carModel.Color;
                car.InsuranceExpiryDate = carModel.InsuranceExpiryDate;
                car.InspectionExpiryDate = carModel.InspectionExpiryDate;
                car.VignetteExpiryDate = carModel.VignetteExpiryDate;

                carModel.ImagePaths ??= new List<string>();

                var existingImagePaths = car.Images.Select(i => i.ImagePath).ToList();

                var imagesToRemove = car.Images.Where(img => !carModel.ImagePaths.Contains(img.ImagePath)).ToList();
                context.CarImages.RemoveRange(imagesToRemove);

                var imagesToAdd = carModel.ImagePaths.Except(existingImagePaths);
                foreach (var imagePath in imagesToAdd)
                {
                    car.Images.Add(new CarImage { ImagePath = imagePath });
                }

                await context.SaveChangesAsync();

                _logger.LogInformation("Car updated for user {UserName}: {CarId} {Brand} {Model} ({LicensePlate})", user.UserName, car.Id, car.Brand, car.Model, car.LicensePlate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update car.");
            }
        }

        public async Task DeleteCarAsync(int id)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Missing HTTP context.");
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(httpContext.User) ?? throw new InvalidOperationException("User must be logged in to delete a car.");

                var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id);
                if (car is null || car.IsDeleted)
                {
                    return;
                }

                if (car.UserId != user.Id)
                {
                    throw new UnauthorizedAccessException("You are not authorized to delete this car.");
                }

                car.IsDeleted = true;
                await context.SaveChangesAsync();

                _logger.LogInformation("Car deleted for user {UserName}: {CarId} {Brand} {Model} ({LicensePlate})", user.UserName, car.Id, car.Brand, car.Model, car.LicensePlate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete car.");
            }
        }
    }
}
