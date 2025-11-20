using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.API.Dto;
using RealEstateAgency.API.Mapper;
using RealEstateAgency.Application.Dto;
using RealEstateAgency.Application.Interfaces.Services;
using RealEstateAgency.Application.Utils;
using RealEstateAgency.Core.Models;

namespace RealEstateAgency.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(UserManager<User> userManager,
    IAccountService accountService,
    ApiMapper mapper,
    IRefreshService refreshService,
    IJwtService jwtService,
    RoleManager<IdentityRole<Guid>> roleManager,
    IAuditService auditService,
    SignInManager<User> signInManager): ControllerBase
{
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            return NotFound();
        }
        
        var res = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        return Ok(res.Errors);
    }
    
    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequestDto request)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            return NotFound();
        }
        
        var res = await userManager.ChangeEmailAsync(user, request.NewEmail,  await userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail));
        return Ok(!res.Errors.Any());
    }
    
    [HttpPost("change-phone")]
    public async Task<IActionResult> ChangePhone([FromBody] ChangePhoneRequestDto request)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            return NotFound();
        }
        
        var res = await userManager.ChangePhoneNumberAsync(user, request.NewPhone, await userManager.GenerateChangePhoneNumberTokenAsync(user, request.NewPhone));
        return Ok(res.Errors);
    }
    
    [HttpPost("get-user-dto-by-id")]
    public async Task<IActionResult> GetUserDtoById([FromBody] Guid userId)
    {
        var user = await accountService.GetUserDtoById(userId);

        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }
    
    [HttpPost("get-stats-by-user-id")]
    public async Task<IActionResult> GetPersonalStatsByUserId([FromBody] Guid userId)
    {
        var stats = await accountService.GetPersonalStatsByUserId(userId);
        return Ok(stats);
    }
    
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