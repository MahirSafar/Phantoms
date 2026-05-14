using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Identity.DTOs;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Entities;
using Phantoms.Infrastructure.Services;

namespace Phantoms.API.Controllers;

[EnableRateLimiting("auth")]
public class AuthController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<AppRole> roleManager,
    IJwtService jwtService,
    IEmailService emailService,
    GoogleTokenValidator googleTokenValidator,
    IConfiguration configuration) : BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new AppUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.UserName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(Result.Failure(result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, Roles.Client);

        _ = emailService.SendWelcomeEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}");

        return Ok(Result.Success("Registration successful."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null || !user.IsActive)
            return Unauthorized(Result.Failure("Invalid credentials."));

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return Unauthorized(Result.Failure("Invalid credentials."));

        return Ok(Result<AuthResponseDto>.Success(await BuildAuthResponse(user)));
    }

    [HttpPost("google-signin")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.IdToken))
            return BadRequest(Result.Failure("Google ID token is required."));

        var payload = await googleTokenValidator.ValidateAsync(dto.IdToken);
        if (payload is null)
            return Unauthorized(Result.Failure("Google token validation failed. Ensure you are sending the id_token (JWT credential) and that the Google Client ID matches the server configuration."));

        var user = await userManager.FindByEmailAsync(payload.Email);
        if (user is null)
        {
            user = new AppUser
            {
                FirstName = payload.GivenName ?? string.Empty,
                LastName = payload.FamilyName ?? string.Empty,
                Email = payload.Email,
                UserName = payload.Email.Split('@')[0],
                ProfileImageUrl = payload.Picture,
                EmailConfirmed = true,
                IsActive = true
            };
            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return BadRequest(Result.Failure(createResult.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, Roles.Client);
            _ = emailService.SendWelcomeEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}");
        }

        if (!user.IsActive)
            return Unauthorized(Result.Failure("Account is deactivated."));

        return Ok(Result<AuthResponseDto>.Success(await BuildAuthResponse(user)));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Ok(Result.Success("If the email exists, a reset code has been sent."));

        var code = Random.Shared.Next(100000, 999999).ToString();
        user.PasswordResetCode = code;
        user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);
        await userManager.UpdateAsync(user);

        _ = emailService.SendPasswordResetCodeEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}", code);

        return Ok(Result.Success("If the email exists, a reset code has been sent."));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return BadRequest(Result.Failure("Invalid request."));

        if (user.PasswordResetCode is null ||
            user.PasswordResetCodeExpiry is null ||
            user.PasswordResetCode != dto.Code ||
            user.PasswordResetCodeExpiry < DateTime.UtcNow)
            return BadRequest(Result.Failure("Invalid or expired reset code."));

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(Result.Failure(result.Errors.Select(e => e.Description)));

        user.PasswordResetCode = null;
        user.PasswordResetCodeExpiry = null;
        await userManager.UpdateAsync(user);

        return Ok(Result.Success("Password reset successfully."));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);

        if (user is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Unauthorized(Result.Failure("Invalid or expired refresh token."));

        return Ok(Result<AuthResponseDto>.Success(await BuildAuthResponse(user)));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized(Result.Failure("User session is invalid."));

        var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(Result.Failure(result.Errors.Select(e => e.Description)));

        return Ok(Result.Success("Password changed successfully."));
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized(Result.Failure("User session is invalid."));

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await userManager.UpdateAsync(user);
        return Ok(Result.Success("Token revoked."));
    }

    private async Task<AuthResponseDto> BuildAuthResponse(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                var claims = await roleManager.GetClaimsAsync(role);
                permissions.AddRange(claims.Where(c => c.Type == "permission").Select(c => c.Value));
            }
        }

        var jwtSettings = configuration.GetSection("JwtSettings");
        var expiryMinutes = jwtSettings.GetValue<int>("AccessTokenExpirationMinutes", 60);

        var accessToken = jwtService.GenerateAccessToken(user, roles, permissions.Distinct().ToList());
        var refreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays", 7));
        await userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes),
            UserId = user.Id.ToString(),
            Email = user.Email ?? string.Empty,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = roles
        };
    }
}
