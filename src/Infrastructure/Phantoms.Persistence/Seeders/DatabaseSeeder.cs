using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Entities;

namespace Phantoms.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<AppDomain>>();
        try
        {
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            // Seed roles
            string[] roleNames = [Roles.Admin, Roles.Client, Roles.Teacher];
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
            }

            // Add all permissions as claims to Admin role
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole is not null)
            {
                var existingClaims = await roleManager.GetClaimsAsync(adminRole);
                foreach (var permission in Permissions.All())
                {
                    if (!existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
                        await roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim("permission", permission));
                }
            }

            // Add basic view permissions to Client role
            var clientRole = await roleManager.FindByNameAsync(Roles.Client);
            if (clientRole is not null)
            {
                var existingClaims = await roleManager.GetClaimsAsync(clientRole);
                string[] clientPermissions = [Permissions.Products.View, Permissions.Events.View, Permissions.Announcements.View];
                foreach (var permission in clientPermissions)
                {
                    if (!existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
                        await roleManager.AddClaimAsync(clientRole, new System.Security.Claims.Claim("permission", permission));
                }
            }

            // Add permissions to Teacher role
            var teacherRole = await roleManager.FindByNameAsync(Roles.Teacher);
            if (teacherRole is not null)
            {
                var existingClaims = await roleManager.GetClaimsAsync(teacherRole);
                string[] teacherPermissions =
                [
                    Permissions.Events.View,
                    Permissions.Events.Create,
                    Permissions.Events.Edit,
                    Permissions.Events.Delete,
                    Permissions.Events.Share,
                    Permissions.Announcements.View,
                    Permissions.Announcements.Create,
                    Permissions.Announcements.Edit,
                    Permissions.Announcements.Delete,
                    Permissions.Announcements.Share
                ];
                foreach (var permission in teacherPermissions)
                {
                    if (!existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
                        await roleManager.AddClaimAsync(teacherRole, new System.Security.Claims.Claim("permission", permission));
                }
            }

            // Seed default admin user
            const string adminEmail = "admin@phantoms.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new AppUser
                {
                    FirstName = "System",
                    LastName = "Admin",
                    Email = adminEmail,
                    UserName = "admin",
                    EmailConfirmed = true,
                    IsActive = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }
            else
            {
                // One-time password reset for existing admin
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                await userManager.ResetPasswordAsync(adminUser, resetToken, "Admin123!");

                if (!adminUser.IsActive)
                {
                    adminUser.IsActive = true;
                    await userManager.UpdateAsync(adminUser);
                }

                if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
