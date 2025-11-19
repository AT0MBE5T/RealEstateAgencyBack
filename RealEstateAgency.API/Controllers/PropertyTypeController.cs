using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.API.Dto;
using RealEstateAgency.Application.Dto;
using RealEstateAgency.Application.Interfaces.Services;
using RealEstateAgency.Application.Services;

namespace RealEstateAgency.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyTypeController(IPropertyTypeService propertyTypeService): ControllerBase
{
    [HttpGet("get-property-types")]
    public async Task<IActionResult> GetPropertyTypes()
    {
        var propertyTypesList = await propertyTypeService.GetAll();
        return Ok(propertyTypesList);
    }
    
    [HttpPost("get-report-by-property-type-id")]
    public async Task<IActionResult> GetReportByPropertyTypeId([FromBody] PropertyTypeStatsRequest request)
    {
        var mapped = new ReportPropertyTypeDto
        {
            PropertyTypeId = request.PropertyTypeId,
            DateFrom = DateTime.Parse(request.DateFrom),
            DateTo = string.IsNullOrEmpty(request.DateTo)
                ? default
                : DateTime.Parse(request.DateTo)
        };
        
        return mapped.DateTo == default
            ? Ok(await propertyTypeService.GetReportByPropertyTypeDate(mapped))
            : Ok(await propertyTypeService.GetReportByPropertyTypeDateSpan(mapped));
    }
}