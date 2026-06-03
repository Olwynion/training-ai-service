namespace Training.AI.Domain.Services;

public interface IAiPlanGenerator
{
    Task<string> GeneratePlanAsync(string prompt, string userExercisesJson, CancellationToken ct = default);
}
