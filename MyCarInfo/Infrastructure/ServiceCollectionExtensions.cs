using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCarInfo.Data;
using MyCarInfo.Models.Options;
using MyCarInfo.Services.Authentication;
using MyCarInfo.Services.Car;
using MyCarInfo.Services.Image;
using MyCarInfo.Services.Notification;

namespace MyCarInfo.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization();
            services.AddCascadingAuthenticationState();
            // Periodically re-validates Identity cookies so the Blazor authentication state stays in sync
            services.AddScoped<IdentityRevalidatingAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<IdentityRevalidatingAuthenticationStateProvider>());

            services.AddHttpContextAccessor();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IImageService, ImageService>();

            services.Configure<ViberOptions>(configuration.GetSection("Viber"));
            services.Configure<NotificationOptions>(configuration.GetSection("Notifications"));

            services.AddHostedService<NotificationBackgroundService>();

            return services;
        }

        public static IServiceCollection AddAppPresentation(this IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            return services;
        }
    }
}
