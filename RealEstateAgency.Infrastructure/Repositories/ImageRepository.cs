using Microsoft.EntityFrameworkCore;
using RealEstateAgency.Application.Interfaces.Repositories;
using RealEstateAgency.Core.Models;
using RealEstateAgency.Infrastructure.Context;

namespace RealEstateAgency.Infrastructure.Repositories;

public class ImageRepository(IDbContextFactory<RealEstateContext> dbContextFactory) : IImageRepository
{
    public async Task<Guid> InsertAsync(Image image)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        var res = await ctx.Images.AddAsync(image);
        await ctx.SaveChangesAsync();
        return res.Entity.Id;
    }
    
    public async Task<bool> UpdateAsync(Guid imageId, byte[] bytes)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        var image = await ctx.Images.FindAsync(imageId);
        if (image == null)
        {
            return false;
        }
        
        image.Bytes = bytes;
        await ctx.SaveChangesAsync();
        return true;
    }
    
    public async Task<byte[]> GetBytesByImageIdAsync(Guid imageId)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync();
        var image = await ctx.Images.FirstOrDefaultAsync(x => x.Id == imageId);
        return image?.Bytes ?? [];
    }
}