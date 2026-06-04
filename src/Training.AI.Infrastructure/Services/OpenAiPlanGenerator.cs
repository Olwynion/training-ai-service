using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Training.AI.Domain.Services;

namespace Training.AI.Infrastructure.Services;

public class OpenAiPlanGenerator(IConfiguration configuration, HttpClient httpClient) : IAiPlanGenerator
{
    private readonly string _apiKey = configuration["OpenAi:ApiKey"] ?? string.Empty;
    private readonly string _model = configuration["OpenAi:Model"] ?? "gpt-4o";

    public async Task<string> GeneratePlanAsync(string prompt, string userExercisesJson, string preferencesContext, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return MockPlanResponse(prompt);
        }

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "Ты профессиональный фитнес-тренер. Составь тренировочный план на основе запроса пользователя и доступных упражнений. Ответ верни в JSON: {\"name\": string, \"days\": [{\"day\": string, \"exercises\": [{\"name\": string, \"sets\": int, \"reps\": int}]}]}. Все названия дней и упражнений на русском языке." },
                new { role = "user", content = $"{preferencesContext}\n\nUser request: {prompt}\n\nAvailable exercises:\n{userExercisesJson}" }
            },
            temperature = 0.7
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent, ct);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(responseBody);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        return content ?? throw new InvalidOperationException("Empty response from OpenAI");
    }

    private static string MockPlanResponse(string prompt)
    {
        return """
            {
                "name": "Тренировка на всё тело",
                "days": [
                    {
                        "day": "День 1 — Грудные и плечи",
                        "exercises": [
                            { "name": "Жим лёжа", "sets": 3, "reps": 10 },
                            { "name": "Жим гантелей сидя", "sets": 3, "reps": 10 }
                        ]
                    },
                    {
                        "day": "День 2 — Спина и бицепс",
                        "exercises": [
                            { "name": "Тяга штанги в наклоне", "sets": 3, "reps": 8 },
                            { "name": "Подтягивания", "sets": 3, "reps": 10 }
                        ]
                    },
                    {
                        "day": "День 3 — Ноги",
                        "exercises": [
                            { "name": "Приседания со штангой", "sets": 3, "reps": 10 },
                            { "name": "Жим ногами", "sets": 3, "reps": 12 }
                        ]
                    }
                ]
            }
            """;
    }
}
