using Moq;
using Training.AI.Domain.Repositories;
using Training.AI.Domain.Services;
using Training.AI.Handlers.GeneratePlan;
using Training.AI.Domain.Entities;

namespace Training.AI.Tests.Handlers;

public class GeneratePlanCommandHandlerTests
{
    private readonly Mock<IGenerationHistoryRepository> _repositoryMock = new();
    private readonly Mock<IAiPlanGenerator> _generatorMock = new();
    private readonly GeneratePlanCommandHandler _handler;

    public GeneratePlanCommandHandlerTests()
    {
        _handler = new GeneratePlanCommandHandler(
            _repositoryMock.Object, _generatorMock.Object);
    }

    [Fact]
    public async Task Handle_GeneratesPlanAndSavesHistory()
    {
        var planJson = """{"name":"Test Plan","days":[]}""";
        _generatorMock
            .Setup(g => g.GeneratePlanAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planJson);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GenerationHistory>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        var command = new GeneratePlanCommand(
            "user1", "build a plan", []);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result.Id);
        Assert.Equal("Test Plan", result.PlanName);
        Assert.Equal(planJson, result.PlanJson);
        _generatorMock.Verify(g => g.GeneratePlanAsync("build a plan", "[]", It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<GenerationHistory>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FallbackPlanNameWhenMissing()
    {
        var planJson = """{"days":[]}""";
        _generatorMock
            .Setup(g => g.GeneratePlanAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(planJson);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GenerationHistory>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2L);

        var command = new GeneratePlanCommand(
            "user1", "build a plan", []);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Training Plan", result.PlanName);
    }
}
