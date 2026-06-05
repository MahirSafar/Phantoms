using System.Security.Claims;
using Phantoms.Application.Common.Interfaces;

namespace Phantoms.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public string? UserId => _user?.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? _user?.FindFirstValue("sub");

    public string? UserName => _user?.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => _user?.IsInRole(role) ?? false;
}
