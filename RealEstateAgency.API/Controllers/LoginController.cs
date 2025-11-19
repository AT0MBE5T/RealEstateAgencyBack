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
public class LoginController(UserManager<User> userManager, SignInManager<User> signInManager,
    IJwtService jwtService, ApiMapper mapper, IRefreshService refreshService, IAuditService auditService): ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        var user = await userManager.FindByNameAsync(loginRequest.Login);
        if (user == null)
        {
            return NotFound("Login not found");
        }

        var response = mapper.LoginRequestToResponse(loginRequest);
        response.Id = user.Id;
        var result = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Incorrect password");
        }

        var refreshToken = await refreshService.GenerateRefreshToken(user.Id);
        var accessToken = await jwtService.GenerateAccessToken(user);
        SetRefreshTokenCookie(refreshToken);
        response.AccessToken = accessToken;

        var auditDto = new AuditDto
        {
            ActionId = Guid.Parse(AuditAction.Login),
            UserId = user.Id,
            Details = $"User {user.UserName} logged in"
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