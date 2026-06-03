using Training.AI.Domain.Entities;

namespace Training.AI.Domain.Repositories;

public interface IGenerationHistoryRepository
{
    Task<long> AddAsync(GenerationHistory history, CancellationToken ct = default);
    Task<IReadOnlyList<GenerationHistory>> GetByUserIdAsync(string userId, CancellationToken ct = default);
}
