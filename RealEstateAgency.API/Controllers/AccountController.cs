using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.API.Dto;
using RealEstateAgency.API.Mapper;
using RealEstateAgency.Application.Interfaces.Services;
using RealEstateAgency.Application.Services;
using RealEstateAgency.Core.Models;

namespace RealEstateAgency.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(UserManager<User> userManager, IUserService userService, ApiMapper mapper): ControllerBase
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
        var user = await userService.GetUserDtoById(userId);

        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }
    
    [HttpPost("get-stats-by-user-id")]
    public async Task<IActionResult> GetPersonalStatsByUserId([FromBody] Guid userId)
    {
        var stats = await userService.GetPersonalStatsByUserId(userId);
        return Ok(stats);
    }
    
    [HttpPost("get-report-by-user-login")]
    public async Task<IActionResult> GetReportByUserLogin([FromBody] ReportUserRequest request)
    {
        var mapped = mapper.ReportUserRequestToReportUserDto(request);

        var res = mapped.DateTo == default
            ? await userService.GetReportByUserLoginDate(mapped)
            : await userService.GetReportByUserLoginDateSpan(mapped);
        
        return res == null
            ? NotFound()
            : Ok(res);
    }
}