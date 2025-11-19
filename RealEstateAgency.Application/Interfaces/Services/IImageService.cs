namespace RealEstateAgency.Application.Interfaces.Services;

public interface IImageService
{
    Task<Guid> InsertAsync(byte[] bytes);
    Task<bool> UpdateAsync(Guid imageId, byte[] bytes);
    Task<byte[]> GetBytesByImageIdAsync(Guid imageId);
}