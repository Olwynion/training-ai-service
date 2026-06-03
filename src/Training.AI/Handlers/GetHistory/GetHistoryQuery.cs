using MediatR;

namespace Training.AI.Handlers.GetHistory;

public record GetHistoryQuery(string UserId) : IRequest<IReadOnlyList<global::Training.AI.Domain.Entities.GenerationHistory>>;
