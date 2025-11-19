using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.API.Dto;
using RealEstateAgency.API.Mapper;
using RealEstateAgency.Application.Dto;
using RealEstateAgency.Application.Interfaces.Services;
using RealEstateAgency.Application.Services;
using RealEstateAgency.Application.Utils;
using RealEstateAgency.Core.Models;

namespace RealEstateAgency.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController(UserManager<User> userManager,
    IJwtService jwtService, IRefreshService refreshService, RoleManager<IdentityRole<Guid>> roleManager,
    ApiMapper mapper, IAuditService auditService): ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
    {
        var existingUser = await userManager.FindByNameAsync(registerRequest.Login);
        if (existingUser != null)
        {
            return Unauthorized("User with this login already exists");
        }

        var userId = Guid.NewGuid();
        var user = mapper.RegisterRequestDtoToUser(registerRequest, DateTime.UtcNow);
        var response = mapper.RegisterRequestToResponse(registerRequest);
        response.Id = userId;
        var result = await userManager.CreateAsync(user, registerRequest.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = "User" });
        }
        
        await userManager.AddToRoleAsync(user, "User");
        var refreshToken = await refreshService.GenerateRefreshToken(user.Id);
        var accessToken = await jwtService.GenerateAccessToken(user);
        SetRefreshTokenCookie(refreshToken);
        response.AccessToken = accessToken;
        var auditDto = new AuditDto
        {
            ActionId = Guid.Parse(AuditAction.Register),
            UserId = user.Id,
            Details = $"User {user.UserName} registered"
        };
        await auditService.InsertAudit(auditDto);
        
        return Ok(response);
    }
    
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = refreshService.GetCookieOptions();
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}