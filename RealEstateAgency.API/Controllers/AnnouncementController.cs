using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateAgency.API.Dto;
using RealEstateAgency.API.Mapper;
using RealEstateAgency.Application.Interfaces.Services;
using RealEstateAgency.Core.Models;

namespace RealEstateAgency.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementController(
    IAnnouncementsService announcementService,
    IImageService imageService,
    IPropertyService propertyService,
    IStatementsService statementService,
    IPaymentService paymentService,
    UserManager<User> userManager,
    ApiMapper mapper): ControllerBase
{

    [HttpGet("get-pages")]
    public async Task<IActionResult> GetPages()
    {
        var pages = await announcementService.GetPages();
        return pages > 0 
            ? Ok(pages) 
            : NoContent();
    }

    [HttpPost("get-announcement-full-by-id")]
    public async Task<IActionResult> GetAnnouncementFullById([FromBody]Guid id)
    {
        var announcementFull = await announcementService.GetAnnouncementFullById(id);

        if (announcementFull == null)
        {
            return NotFound();
        }
        
        return Ok(announcementFull);
    }
    
    [HttpPost("get-announcement-for-edit")]
    public async Task<IActionResult> GetAnnouncementForEdit([FromBody]Guid id)
    {
        var announcementData = await announcementService.GetAnnouncementForEditByIdAsync(id);

        if (announcementData == null)
        {
            return NotFound();
        }
        
        return Ok(announcementData);
    }
    
    [HttpPost("add-announcement")]
    public async Task<IActionResult> AddAnnouncement([FromForm] AnnouncementRequest request)
    {
        var photo = request.Photo;
        
        if (photo == null || photo.Length == 0)
            return BadRequest("No file uploaded");

        await using var ms = new MemoryStream();
        await photo.CopyToAsync(ms);
        var bytes = ms.ToArray();
        var imageId = await imageService.InsertAsync(bytes);
        var property = mapper.AnnouncementRequestToPropertyDto(request, imageId);
        var propertyId = await propertyService.AddProperty(property);
        var statement = mapper.AnnouncementRequestToStatementDto(request, propertyId, DateTime.UtcNow);
        var statementId = await statementService.AddStatementAsync(statement);

        if (statementId == null)
        {
            return NotFound();
        }

        var announcement = mapper.AnnouncementRequestToAnnouncementDto(request, (Guid)statementId, true);
        var announcementId = await announcementService.AddAnnouncement(request.UserId, announcement);
        
        return Ok(announcementId);
    }
    
    [HttpPost("update-announcement")]
    public async Task<IActionResult> UpdateAnnouncement([FromForm] AnnouncementEditRequest request)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var photo = request.Photo;

        byte[] bytes = [];
        if (photo != null && photo.Length != 0)
        {
            await using var ms = new MemoryStream();
            await photo.CopyToAsync(ms);
            bytes = ms.ToArray();
        }

        var oldImage = await announcementService.GetBytesByAnnouncementIdAsync(request.AnnouncementId);
        var imageId = await announcementService.GetImageIdByAnnouncementIdAsync(request.AnnouncementId);
        var statementId = await announcementService.GetStatementIdByAnnouncementIdAsync(request.AnnouncementId);
        var propertyId = await announcementService.GetPropertyIdByAnnouncementIdAsync(request.AnnouncementId);

        if (statementId == null || propertyId == null)
        {
            return NotFound();
        }
        
        if (oldImage != bytes && bytes.Length != 0)
        {
            var res = await imageService.UpdateAsync(imageId, bytes);
            if (!res)
            {
                return NotFound();
            }
        }

        var property = mapper.AnnouncementEditRequestToPropertyDto(request, imageId);
        var propertyRes = await propertyService.UpdatePropertyAsync((Guid)propertyId, property);
        
        if (!propertyRes)
        {
            return NotFound();
        }
        
        var statement = mapper.AnnouncementEditRequestToStatementDto(request, (Guid)propertyId, DateTime.UtcNow);
        var statementRes = await statementService.UpdateStatementAsync((Guid)statementId, user.Id, statement);
        
        if (!statementRes)
        {
            return NotFound();
        }
        
        var announcement = mapper.AnnouncementEditRequestToAnnouncementDto(request, (Guid)statementId, true);
        var announcementId = await announcementService.UpdateAnnouncementAsync(request.AnnouncementId, user.Id, announcement);
        
        return Ok(announcementId);
    }

    [HttpPost("delete-announcement-by-id")]
    public async Task<IActionResult> DeleteAnnouncement([FromBody] Guid announcementId)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        return await announcementService.DeleteAnnouncementAsync(announcementId, user.Id)
            ?  Ok()
            : NotFound();
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequestDto request)
    {
        var announcements = await announcementService.GetSearchDataPaginated(request.Text, request.Filters, request.SortId, request.Page);
        var res = mapper.ListAnnouncementsShortAndPagesToListAnnouncementResponse(announcements.Data);
        var response = mapper.ToAnnouncementsResponseAndPages(
            announcements,
            res
        );
        
        return Ok(response);
    }

    [HttpPost("buy-property-by-announcement_id")]
    public async Task<IActionResult> BuyPropertyByAnnouncement([FromBody] BuyRequest request)
    {
        var paymentDto = mapper.BuyRequestToPaymentDto(request);
        var paymentId = await paymentService.InsertPayment(paymentDto);
        
        return paymentId == Guid.Empty
            ? NotFound()
            : Ok(paymentId);
    }
    
    [HttpPost("get-bought-by-user-id")]
    public async Task<IActionResult> GetBoughtAnnouncementsByUserId([FromBody] AnnouncementsStatRequestDto request)
    {
        var announcements = await announcementService.GetBoughtAnnouncementsByUserId(request.UserId, request.Page);
        var res = mapper.ListAnnouncementsShortAndPagesToListAnnouncementResponse(announcements.Data);
        var response = mapper.ToAnnouncementsResponseAndPages(
            announcements,
            res
        );
        
        return Ok(response);
    }
    
    [HttpPost("get-sold-by-user-id")]
    public async Task<IActionResult> GetSoldAnnouncementsByUserId([FromBody] AnnouncementsStatRequestDto request)
    {
        var announcements = await announcementService.GetSoldAnnouncementsByUserId(request.UserId, request.Page);
        var res = mapper.ListAnnouncementsShortAndPagesToListAnnouncementResponse(announcements.Data);
        var response = mapper.ToAnnouncementsResponseAndPages(
            announcements,
            res
        );
        
        return Ok(response);
    }
    
    [HttpPost("get-placed-by-user-id")]
    public async Task<IActionResult> GetPlacedAnnouncementsByUserId([FromBody] AnnouncementsStatRequestDto request)
    {
        var announcements = await announcementService.GetPlacedAnnouncementsByUserId(request.UserId, request.Page);
        var res = mapper.ListAnnouncementsShortAndPagesToListAnnouncementResponse(announcements.Data);
        var response = mapper.ToAnnouncementsResponseAndPages(
            announcements,
            res
        );
        
        return Ok(response);
    }
    
    [HttpPost("switch-verification")]
    public async Task<IActionResult> SwitchVerify([FromBody] Guid announcementId)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var res = await announcementService.SwitchVerificateAnnouncement(announcementId, user.Id);
        
        return res
                ? Ok()
                : NotFound();
    }
}