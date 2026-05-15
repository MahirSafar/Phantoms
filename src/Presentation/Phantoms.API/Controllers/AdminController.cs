using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phantoms.API.Authorization;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Identity.DTOs;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Entities;

namespace Phantoms.API.Controllers;

[Authorize]
public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [HasPermission(Permissions.Users.View)]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userManager.Users.ToListAsync();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            result.Add(new UserDto
            {
                Id = user.Id.ToString(),
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                IsActive = user.IsActive,
                Roles = await userManager.GetRolesAsync(user)
            });
        }
        return Ok(Result<List<UserDto>>.Success(result));
    }

    [HasPermission(Permissions.Users.Edit)]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user is null) return NotFound(Result.Failure("User not found."));

        if (!await userManager.IsInRoleAsync(user, dto.RoleName))
            await userManager.AddToRoleAsync(user, dto.RoleName);

        return Ok(Result.Success($"Role '{dto.RoleName}' assigned to user."));
    }

    [HasPermission(Permissions.Users.Edit)]
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user is null) return NotFound(Result.Failure("User not found."));

        await userManager.RemoveFromRoleAsync(user, dto.RoleName);
        return Ok(Result.Success($"Role '{dto.RoleName}' removed from user."));
    }

    [HasPermission(Permissions.Users.Edit)]
    [HttpPatch("users/{userId}/toggle-active")]
    public async Task<IActionResult> ToggleActive(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(Result.Failure("User not found."));

        user.IsActive = !user.IsActive;
        await userManager.UpdateAsync(user);
        return Ok(Result.Success($"User is now {(user.IsActive ? "active" : "inactive")}."));
    }

    [HasPermission(Permissions.Users.Delete)]
    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound(Result.Failure("User not found."));

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(Result.Failure(result.Errors.Select(e => e.Description)));

        return Ok(Result.Success("User deleted."));
    }
}
