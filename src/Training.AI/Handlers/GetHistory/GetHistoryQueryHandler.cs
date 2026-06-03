using MediatR;
using Training.AI.Domain.Repositories;

namespace Training.AI.Handlers.GetHistory;

public class GetHistoryQueryHandler(IGenerationHistoryRepository repository)
    : IRequestHandler<GetHistoryQuery, IReadOnlyList<global::Training.AI.Domain.Entities.GenerationHistory>>
{
    public async Task<IReadOnlyList<global::Training.AI.Domain.Entities.GenerationHistory>> Handle(
        GetHistoryQuery request, CancellationToken ct)
    {
        return await repository.GetByUserIdAsync(request.UserId, ct);
    }
}
