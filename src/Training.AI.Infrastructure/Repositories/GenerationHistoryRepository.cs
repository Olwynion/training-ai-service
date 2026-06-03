using Dapper;
using Training.AI.Domain.Entities;
using Training.AI.Domain.Repositories;

namespace Training.AI.Infrastructure.Repositories;

public class GenerationHistoryRepository(IDbConnectionFactory connectionFactory) : IGenerationHistoryRepository
{
    public async Task<long> AddAsync(GenerationHistory history, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var id = await conn.QuerySingleAsync<long>(
            new CommandDefinition(
                commandText: @"
                    INSERT INTO generation_history (user_id, prompt, plan_name, plan_json)
                    VALUES (@UserId, @Prompt, @PlanName, @PlanJson)
                    RETURNING id",
                parameters: new { history.UserId, history.Prompt, history.PlanName, history.PlanJson },
                cancellationToken: ct));

        return id;
    }

    public async Task<IReadOnlyList<GenerationHistory>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var rows = await conn.QueryAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM generation_history WHERE user_id = @UserId ORDER BY created_at DESC",
                parameters: new { UserId = userId },
                cancellationToken: ct));

        return rows.Select(row => GenerationHistory.Hydrate(
            (long)row.id,
            (string)row.user_id,
            (string)row.prompt,
            (string)row.plan_name,
            (string)row.plan_json,
            (DateTime)row.created_at
        )).ToList();
    }
}
