using MediatR;
using Training.AI.Proto;
using Training.Training.Proto;

namespace Training.AI.Handlers.GeneratePlan;

public record GeneratePlanCommand(
    string UserId,
    string Prompt,
    List<ExerciseTemplate> Exercises,
    int DaysPerWeek,
    string ProgramType,
    MuscleGroup FocusGroup) : IRequest<GeneratePlanResult>;
