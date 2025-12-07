using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyCarInfo.Data;

namespace MyCarInfo.Services.Authentication;

/// <summary>
/// Server authentication state provider that periodically revalidates the current Identity user
/// so the Blazor authentication state stays in sync with the cookie on the server.
/// </summary>
public class IdentityRevalidatingAuthenticationStateProvider
    : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IdentityOptions _options;

    public IdentityRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _options = optionsAccessor.Value;
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        var user = authenticationState.User;
        if (user.Identity?.IsAuthenticated is not true)
        {
            return false;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var userId = userManager.GetUserId(user);
        if (userId is null)
        {
            return false;
        }

        var reloadedUser = await userManager.FindByIdAsync(userId);
        if (reloadedUser is null)
        {
            return false;
        }

        var securityStampStore = userManager as IUserSecurityStampStore<ApplicationUser>;
        if (securityStampStore is null)
        {
            return true;
        }

        var principalStamp = user.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
        var userStamp = await securityStampStore.GetSecurityStampAsync(reloadedUser, cancellationToken);

        return principalStamp == userStamp;
    }
}