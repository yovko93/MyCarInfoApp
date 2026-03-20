using Microsoft.AspNetCore.Identity;
using MyCarInfo.Data;
using MyCarInfo.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppPresentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/logout", async (SignInManager<ApplicationUser> signInManager, HttpContext httpContext, ILogger<Program> logger) =>
{
    var username = httpContext.User.Identity?.Name ?? "unknown";
    await signInManager.SignOutAsync();
    logger.LogInformation("User logged out: {Username}", username);
    return Results.Redirect("/");
}).RequireAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
