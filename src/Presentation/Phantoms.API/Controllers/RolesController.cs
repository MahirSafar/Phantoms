using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Phantoms.API.Authorization;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Identity.DTOs;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Entities;

namespace Phantoms.API.Controllers;

[Authorize]
public class RolesController(RoleManager<AppRole> roleManager) : BaseApiController
{
    [HasPermission(Permissions.Roles.View)]
    [HttpGet]
    public IActionResult GetAll()
    {
        var roles = roleManager.Roles.Select(r => new { r.Id, r.Name, r.Description }).ToList();
        return Ok(Result<object>.Success(roles));
    }

    [HasPermission(Permissions.Roles.Manage)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        if (await roleManager.RoleExistsAsync(dto.Name))
            return BadRequest(Result.Failure($"Role '{dto.Name}' already exists."));

        var role = new AppRole { Name = dto.Name, Description = dto.Description };
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return BadRequest(Result.Failure(result.Errors.Select(e => e.Description)));

        foreach (var permission in dto.Permissions)
            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", permission));

        return Ok(Result.Success("Role created successfully."));
    }

    [HasPermission(Permissions.Roles.Manage)]
    [HttpPost("{roleName}/permissions")]
    public async Task<IActionResult> AddPermissions(string roleName, [FromBody] List<string> permissions)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null) return NotFound(Result.Failure("Role not found."));

        var existing = await roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!existing.Any(c => c.Type == "permission" && c.Value == permission))
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", permission));
        }

        return Ok(Result.Success("Permissions added."));
    }

    [HasPermission(Permissions.Roles.Manage)]
    [HttpDelete("{roleName}/permissions")]
    public async Task<IActionResult> RemovePermissions(string roleName, [FromBody] List<string> permissions)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null) return NotFound(Result.Failure("Role not found."));

        foreach (var permission in permissions)
        {
            var claim = (await roleManager.GetClaimsAsync(role))
                .FirstOrDefault(c => c.Type == "permission" && c.Value == permission);
            if (claim is not null)
                await roleManager.RemoveClaimAsync(role, claim);
        }

        return Ok(Result.Success("Permissions removed."));
    }

    [HasPermission(Permissions.Roles.View)]
    [HttpGet("{roleName}/permissions")]
    public async Task<IActionResult> GetPermissions(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null) return NotFound(Result.Failure("Role not found."));

        var claims = await roleManager.GetClaimsAsync(role);
        var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();
        return Ok(Result<List<string>>.Success(permissions));
    }

    [HasPermission(Permissions.Roles.Manage)]
    [HttpDelete("{roleName}")]
    public async Task<IActionResult> Delete(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null) return NotFound(Result.Failure("Role not found."));

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return BadRequest(Result.Failure(result.Errors.Select(e => e.Description)));

        return Ok(Result.Success("Role deleted."));
    }

    [HasPermission(Permissions.Roles.View)]
    [HttpGet("permissions")]
    public IActionResult GetAllPermissions()
    {
        var permissions = typeof(Permissions)
            .GetNestedTypes()
            .ToDictionary(
                module => module.Name,
                module => module.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
                                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                                .Select(f => (string)f.GetRawConstantValue()!)
                                .ToList()
            );

        return Ok(Result<object>.Success(permissions));
    }
}
