using RealEstateAgency.Core.Models;

namespace RealEstateAgency.Application.Interfaces.Repositories;

public interface IAnswersRepository
{
    Task<Answer?> GetAllByQuestionIdAsync(Guid id);
    Task<Guid> InsertAsync(Answer answer);
    Task<bool> DeleteByIdAsync(Guid id);
}