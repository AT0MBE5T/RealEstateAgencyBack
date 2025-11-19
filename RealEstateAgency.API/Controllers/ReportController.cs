using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.Application.Interfaces.Services;
using RealEstateAgency.Application.Services;

namespace RealEstateAgency.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(IReportsService reportService) : ControllerBase
{
    [HttpGet("get-general-report")]
    public async Task<IActionResult> GetGeneralReport()
    {
        var res = await reportService.GetGeneralReport();
        return Ok(res);
    }
}