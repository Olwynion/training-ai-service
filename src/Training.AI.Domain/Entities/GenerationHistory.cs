namespace Training.AI.Domain.Entities;

public class GenerationHistory
{
    public long Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public string Prompt { get; private set; } = null!;
    public string PlanName { get; private set; } = null!;
    public string PlanJson { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private GenerationHistory() { }

    public static GenerationHistory Create(string userId, string prompt, string planName, string planJson)
    {
        return new GenerationHistory
        {
            UserId = userId,
            Prompt = prompt,
            PlanName = planName,
            PlanJson = planJson,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static GenerationHistory Hydrate(long id, string userId, string prompt, string planName, string planJson, DateTime createdAt)
    {
        return new GenerationHistory
        {
            Id = id, UserId = userId, Prompt = prompt,
            PlanName = planName, PlanJson = planJson, CreatedAt = createdAt
        };
    }

    public void SetId(long id) => Id = id;
}
