using System.Text.Json;
using MediatR;
using Training.AI.Domain.Repositories;
using Training.AI.Domain.Services;

namespace Training.AI.Handlers.GeneratePlan;

public class GeneratePlanCommandHandler(
    IGenerationHistoryRepository repository,
    IAiPlanGenerator planGenerator) : IRequestHandler<GeneratePlanCommand, GeneratePlanResult>
{
    public async Task<GeneratePlanResult> Handle(GeneratePlanCommand request, CancellationToken ct)
    {
        var exercisesJson = JsonSerializer.Serialize(request.Exercises);
        var planJson = await planGenerator.GeneratePlanAsync(request.Prompt, exercisesJson, ct);

        using var planDoc = JsonDocument.Parse(planJson);
        var planName = planDoc.RootElement.TryGetProperty("name", out var nameProp)
            ? nameProp.GetString() ?? "Training Plan"
            : "Training Plan";

        var history = global::Training.AI.Domain.Entities.GenerationHistory.Create(request.UserId, request.Prompt, planName, planJson);
        var id = await repository.AddAsync(history, ct);
        history.SetId(id);

        return new GeneratePlanResult(id, planName, planJson, history.CreatedAt);
    }
}
