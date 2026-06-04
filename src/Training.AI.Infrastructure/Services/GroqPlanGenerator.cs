using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Training.AI.Domain.Services;

namespace Training.AI.Infrastructure.Services;

public class GroqPlanGenerator(IConfiguration configuration, HttpClient httpClient) : IAiPlanGenerator
{
    private readonly string _apiKey = configuration["Groq:ApiKey"] ?? string.Empty;
    private readonly string _model = configuration["Groq:Model"] ?? "llama-3.3-70b-versatile";

    public async Task<string> GeneratePlanAsync(string prompt, string userExercisesJson, string preferencesContext, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
            return MockPlanResponse();

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "Ты профессиональный фитнес-тренер. Составь тренировочный план СТРОГО по предпочтениям пользователя. Используй ТОЛЬКО переданные доступные упражнения. Ответ верни ТОЛЬКО в виде JSON, без пояснений, без markdown. Формат: {\"name\": string, \"days\": [{\"day\": string, \"focus\": int, \"exercises\": [{\"name\": string, \"sets\": int, \"reps\": int}]}]}. focus — номер группы мышц: 0=равномерно,1=грудные,2=спина,3=ноги,4=плечи,5=бицепс,6=трицепс,7=пресс. daysPerWeek определяет количество дней в плане. Все названия дней и упражнений на русском языке." },
                new { role = "user", content = $"{preferencesContext}\n\nUser request: {prompt}\n\nAvailable exercises:\n{userExercisesJson}" }
            },
            temperature = 0.7
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", jsonContent, ct);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(responseBody);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()
            ?? throw new InvalidOperationException("Empty response from Groq");

        return CleanJsonResponse(content);
    }

    private static string CleanJsonResponse(string raw)
    {
        var trimmed = raw.Trim();
        if (trimmed.StartsWith("```"))
        {
            var start = trimmed.IndexOf('\n') + 1;
            var end = trimmed.LastIndexOf("```");
            trimmed = trimmed[start..end].Trim();
        }
        var jsonStart = trimmed.IndexOf('{');
        var jsonEnd = trimmed.LastIndexOf('}');
        return jsonStart >= 0 && jsonEnd > jsonStart
            ? trimmed[jsonStart..(jsonEnd + 1)]
            : trimmed;
    }

    private static string MockPlanResponse()
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
