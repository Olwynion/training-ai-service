using MediatR;
using Training.AI.Proto;

namespace Training.AI.Handlers.GeneratePlan;

public record GeneratePlanCommand(string UserId, string Prompt, List<ExerciseTemplate> Exercises) : IRequest<GeneratePlanResult>;
