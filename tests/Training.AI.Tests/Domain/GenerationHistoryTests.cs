using Training.AI.Domain.Entities;

namespace Training.AI.Tests.Domain;

public class GenerationHistoryTests
{
    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var history = GenerationHistory.Create("user1", "test prompt", "My Plan", "{}");

        Assert.Equal("user1", history.UserId);
        Assert.Equal("test prompt", history.Prompt);
        Assert.Equal("My Plan", history.PlanName);
        Assert.Equal("{}", history.PlanJson);
        Assert.Equal(0, history.Id);
        Assert.NotEqual(default, history.CreatedAt);
    }

    [Fact]
    public void Hydrate_SetsAllProperties()
    {
        var createdAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var history = GenerationHistory.Hydrate(42, "user1", "prompt", "Plan", "{}", createdAt);

        Assert.Equal(42, history.Id);
        Assert.Equal("user1", history.UserId);
        Assert.Equal("prompt", history.Prompt);
        Assert.Equal("Plan", history.PlanName);
        Assert.Equal("{}", history.PlanJson);
        Assert.Equal(createdAt, history.CreatedAt);
    }

    [Fact]
    public void Id_IsZeroAfterCreate()
    {
        var history = GenerationHistory.Create("user1", "prompt", "Plan", "{}");
        Assert.Equal(0, history.Id);
    }

    [Fact]
    public void SetId_UpdatesId()
    {
        var history = GenerationHistory.Create("user1", "prompt", "Plan", "{}");
        history.SetId(100);
        Assert.Equal(100, history.Id);
    }
}
