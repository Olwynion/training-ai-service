using Grpc.Core;
using MediatR;
using Training.AI.Handlers.GeneratePlan;
using Training.AI.Handlers.GetHistory;
using Training.AI.Proto;

namespace Training.AI.Grpc;

public class AiGrpcService(IMediator mediator) : AiService.AiServiceBase
{
    public override async Task<GeneratePlanResponse> GeneratePlan(GeneratePlanRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new GeneratePlanCommand(request.UserId, request.Prompt, request.Exercises.ToList()),
            context.CancellationToken);

        return new GeneratePlanResponse
        {
            Id = result.Id,
            PlanName = result.PlanName,
            PlanJson = result.PlanJson,
            CreatedAt = result.CreatedAt.ToString("O")
        };
    }

    public override async Task<GetGenerationHistoryResponse> GetGenerationHistory(GetGenerationHistoryRequest request, ServerCallContext context)
    {
        var items = await mediator.Send(
            new GetHistoryQuery(request.UserId),
            context.CancellationToken);

        var response = new GetGenerationHistoryResponse();
        response.Items.AddRange(items.Select(i => new GenerationHistoryItem
        {
            Id = i.Id,
            PlanName = i.PlanName,
            Prompt = i.Prompt,
            CreatedAt = i.CreatedAt.ToString("O")
        }));
        return response;
    }
}
