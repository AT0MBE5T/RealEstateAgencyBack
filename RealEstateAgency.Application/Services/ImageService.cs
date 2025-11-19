using RealEstateAgency.Application.Interfaces.Repositories;
using RealEstateAgency.Application.Interfaces.Services;
using RealEstateAgency.Core.Models;

namespace RealEstateAgency.Application.Services;

public class ImageService(IImageRepository repository) : IImageService
{
    public async Task<Guid> InsertAsync(byte[] bytes)
    {
        try
        {
            var newId = Guid.NewGuid();
            var image = new Image
            {
                Id = newId,
                Bytes = bytes
            };
            await repository.InsertAsync(image);
            return newId;
        }
        catch
        {
            return Guid.Empty;
        }
    }
    
    public async Task<bool> UpdateAsync(Guid imageId, byte[] bytes)
    {
        try
        {
            var res = await repository.UpdateAsync(imageId, bytes);
            return res;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<byte[]> GetBytesByImageIdAsync(Guid imageId)
    {
        var result = await repository.GetBytesByImageIdAsync(imageId);
        return result;
    }
}