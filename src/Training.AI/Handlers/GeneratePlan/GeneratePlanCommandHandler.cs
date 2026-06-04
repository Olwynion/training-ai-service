using System.Text.Json;
using MediatR;
using Training.AI.Domain.Repositories;
using Training.AI.Domain.Services;
using Training.Training.Proto;

namespace Training.AI.Handlers.GeneratePlan;

public class GeneratePlanCommandHandler(
    IGenerationHistoryRepository repository,
    IAiPlanGenerator planGenerator) : IRequestHandler<GeneratePlanCommand, GeneratePlanResult>
{
    public async Task<GeneratePlanResult> Handle(GeneratePlanCommand request, CancellationToken ct)
    {
        var exercisesJson = JsonSerializer.Serialize(request.Exercises);
        var preferencesContext = GetPreferencesContext(request);
        var planJson = await planGenerator.GeneratePlanAsync(request.Prompt, exercisesJson, preferencesContext, ct);

        using var planDoc = JsonDocument.Parse(planJson);
        var planName = planDoc.RootElement.TryGetProperty("name", out var nameProp)
            ? nameProp.GetString() ?? "Training Plan"
            : "Training Plan";

        var history = global::Training.AI.Domain.Entities.GenerationHistory.Create(request.UserId, request.Prompt, planName, planJson);
        var id = await repository.AddAsync(history, ct);
        history.SetId(id);

        return new GeneratePlanResult(id, planName, planJson, history.CreatedAt);
    }

    private static string GetPreferencesContext(GeneratePlanCommand request)
    {
        var focusGroupStr = request.FocusGroup switch
        {
            MuscleGroup.Chest => "грудные",
            MuscleGroup.Back => "спина",
            MuscleGroup.Legs => "ноги",
            MuscleGroup.Shoulders => "плечи",
            MuscleGroup.Biceps => "бицепс",
            MuscleGroup.Triceps => "трицепс",
            MuscleGroup.Core => "пресс",
            _ => "равномерно"
        };

        var programTypeStr = request.ProgramType == "fullbody" ? "фулбоди" : "сплит";
        return $"Предпочтения: программа: {programTypeStr}, фокус: {focusGroupStr}.";
    }
}
