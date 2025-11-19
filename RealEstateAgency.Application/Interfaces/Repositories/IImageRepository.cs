using RealEstateAgency.Core.Models;

namespace RealEstateAgency.Application.Interfaces.Repositories;

public interface IImageRepository
{
    Task<Guid> InsertAsync(Image image);
    Task<bool> UpdateAsync(Guid imageId, byte[] bytes);
    Task<byte[]> GetBytesByImageIdAsync(Guid imageId);
}