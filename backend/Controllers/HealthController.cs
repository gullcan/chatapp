using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("healthz")]
public class HealthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public HealthController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var aiBaseUrl = _config["AI_SERVICE_URL"] ?? Environment.GetEnvironmentVariable("AI_SERVICE_URL");
        if (string.IsNullOrWhiteSpace(aiBaseUrl))
        {
            return StatusCode(500, new { status = "fail", reason = "AI_SERVICE_URL not configured" });
        }

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(20);

        var payload = new { data = new string[] { "health check" } };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var baseUrl = aiBaseUrl.TrimEnd('/');
        var primary = baseUrl + "/api/predict";
        var fallback = baseUrl + "/run/predict";

        var sw = Stopwatch.StartNew();
        try
        {
            var resp = await client.PostAsync(primary, content);
            if (!resp.IsSuccessStatusCode)
            {
                // try fallback
                resp = await client.PostAsync(fallback, content);
            }
            sw.Stop();

            var body = await resp.Content.ReadAsStringAsync();
            var ok = resp.IsSuccessStatusCode;
            return Ok(new
            {
                status = ok ? "ok" : "degraded",
                latencyMs = sw.ElapsedMilliseconds,
                endpoint = ok ? (resp.RequestMessage?.RequestUri?.ToString() ?? primary) : primary,
                code = (int)resp.StatusCode,
                body = body
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            return StatusCode(502, new
            {
                status = "fail",
                latencyMs = sw.ElapsedMilliseconds,
                error = ex.Message,
                tried = new[] { primary, fallback }
            });
        }
    }
}
