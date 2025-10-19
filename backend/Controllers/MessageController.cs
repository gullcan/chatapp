using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatBackend.Data;
using ChatBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public MessageController(IHttpClientFactory httpClientFactory, AppDbContext db, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _config = config;
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages([FromQuery] int limit = 50)
    {
        if (limit <= 0 || limit > 200) limit = 50;
        var items = await _db.Messages
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .Select(m => new {
                id = m.Id,
                username = m.Username,
                text = m.Text,
                sentiment = m.Sentiment,
                createdAt = m.CreatedAt
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeMessage([FromBody] MessageRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { error = "Text is required" });
        }

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(60);

        // AI service base url from env: AI_SERVICE_URL (e.g., https://<username>-ai-service.hf.space)
        var aiBaseUrl = _config["AI_SERVICE_URL"] ?? Environment.GetEnvironmentVariable("AI_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(aiBaseUrl))
        {
            return StatusCode(500, new { error = "AI_SERVICE_URL is not configured" });
        }

        var payload = new { data = new string[] { request.Text } };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var baseUrl = aiBaseUrl.TrimEnd('/');
        var firstUrl = baseUrl + "/api/predict";
        var secondUrl = baseUrl + "/run/predict"; // fallback for older Gradio

        HttpResponseMessage response;
        string json;

        // Try primary endpoint
        response = await client.PostAsync(firstUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            // Try fallback endpoint
            var fallback = await client.PostAsync(secondUrl, content);
            if (!fallback.IsSuccessStatusCode)
            {
                var body1 = await response.Content.ReadAsStringAsync();
                var body2 = await fallback.Content.ReadAsStringAsync();
                return StatusCode((int)fallback.StatusCode, new { error = "AI service unreachable", primary = new { url = firstUrl, status = (int)response.StatusCode, body = body1 }, fallback = new { url = secondUrl, status = (int)fallback.StatusCode, body = body2 } });
            }
            response = fallback;
        }

        json = await response.Content.ReadAsStringAsync();

        // Gradio returns {"data": ["label"]}
        string sentiment = "neutral";
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("data", out var dataEl) && dataEl.ValueKind == JsonValueKind.Array && dataEl.GetArrayLength() > 0)
            {
                sentiment = dataEl[0].GetString() ?? "neutral";
            }
        }
        catch
        {
            // fallback keeps sentiment as neutral
        }

        // Save to DB
        var entity = new Message
        {
            Username = request.Username ?? "guest",
            Text = request.Text,
            Sentiment = sentiment
        };
        _db.Messages.Add(entity);
        await _db.SaveChangesAsync();

        // Return structured response
        return Ok(new
        {
            id = entity.Id,
            username = entity.Username,
            text = entity.Text,
            sentiment = entity.Sentiment,
            createdAt = entity.CreatedAt
        });
    }
}

public class MessageRequest
{
    public string? Username { get; set; }
    public string Text { get; set; } = string.Empty;
}
