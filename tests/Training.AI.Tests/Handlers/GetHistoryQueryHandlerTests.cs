using Moq;
using Training.AI.Domain.Repositories;
using Training.AI.Handlers.GetHistory;
using Training.AI.Domain.Entities;

namespace Training.AI.Tests.Handlers;

public class GetHistoryQueryHandlerTests
{
    private readonly Mock<IGenerationHistoryRepository> _repositoryMock = new();
    private readonly GetHistoryQueryHandler _handler;

    public GetHistoryQueryHandlerTests()
    {
        _handler = new GetHistoryQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsHistoryList()
    {
        var items = new List<GenerationHistory>
        {
            GenerationHistory.Hydrate(1, "user1", "prompt1", "Plan1", "{}", DateTime.UtcNow),
            GenerationHistory.Hydrate(2, "user1", "prompt2", "Plan2", "{}", DateTime.UtcNow)
        };

        _repositoryMock
            .Setup(r => r.GetByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var query = new GetHistoryQuery("user1");
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Plan1", result[0].PlanName);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyListWhenNoHistory()
    {
        _repositoryMock
            .Setup(r => r.GetByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GenerationHistory>());

        var query = new GetHistoryQuery("user1");
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result);
    }
}
