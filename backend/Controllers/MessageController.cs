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
        client.Timeout = TimeSpan.FromSeconds(90); // Increased for queue polling

        // AI service base url from env: AI_SERVICE_URL (e.g., https://<username>-ai-service.hf.space)
        var aiBaseUrl = _config["AI_SERVICE_URL"] ?? Environment.GetEnvironmentVariable("AI_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(aiBaseUrl))
        {
            return StatusCode(500, new { error = "AI_SERVICE_URL is not configured" });
        }

        // Add HuggingFace API token for authentication
        var hfToken = _config["HF_TOKEN"] ?? Environment.GetEnvironmentVariable("HF_TOKEN");
        if (!string.IsNullOrWhiteSpace(hfToken))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {hfToken}");
        }

        // Use HuggingFace Inference API directly instead of Space
        // This is more reliable and doesn't require dealing with Gradio's queue system
        var inferenceUrl = "https://api-inference.huggingface.co/models/cardiffnlp/twitter-roberta-base-sentiment-latest";
        var payload = new { inputs = request.Text };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        
        string sentiment = "neutral";
        
        try
        {
            var response = await client.PostAsync(inferenceUrl, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                // Fallback to neutral on error
                sentiment = "neutral";
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                
                // HF Inference API returns: [[{"label": "positive", "score": 0.98}]]
                try
                {
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                    {
                        var firstArray = doc.RootElement[0];
                        if (firstArray.ValueKind == JsonValueKind.Array && firstArray.GetArrayLength() > 0)
                        {
                            var result = firstArray[0];
                            if (result.TryGetProperty("label", out var labelEl))
                            {
                                var label = labelEl.GetString() ?? "neutral";
                                label = label.ToLower();
                                
                                // Normalize label
                                if (label.Contains("pos"))
                                {
                                    sentiment = "positive";
                                }
                                else if (label.Contains("neg"))
                                {
                                    sentiment = "negative";
                                }
                                else
                                {
                                    sentiment = "neutral";
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // fallback keeps sentiment as neutral
                }
            }
        }
        catch (Exception ex)
        {
            // Fallback to neutral on any error
            sentiment = "neutral";
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
