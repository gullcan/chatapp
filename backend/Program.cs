using ChatBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // 🔹 Controller ekle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use connection string from env/config if provided, else default to a writeable path (/data/chat.db for Render)
var connString = builder.Configuration.GetConnectionString("Default")
                 ?? builder.Configuration["ConnectionStrings:Default"]
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                 ?? "Data Source=/data/chat.db";

// Ensure SQLite directory exists if a file path is provided
static void EnsureSqliteDirectory(string cs)
{
    const string key = "Data Source=";
    var idx = cs.IndexOf(key, StringComparison.OrdinalIgnoreCase);
    if (idx >= 0)
    {
        var path = cs.Substring(idx + key.Length).Trim();
        // If connection string contains additional segments (e.g., ";Cache=Shared"), split them
        var semicolonIdx = path.IndexOf(';');
        if (semicolonIdx >= 0)
            path = path.Substring(0, semicolonIdx);
        try
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        catch
        {
            // Best-effort: if we can't create directory, let EF throw a clear error later
        }
    }
}

EnsureSqliteDirectory(connString);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connString));

// Configure CORS for the deployed frontend origin(s)
// FRONTEND_ORIGIN may be a single origin or comma-separated list
var frontendOriginCsv = builder.Configuration["FRONTEND_ORIGIN"]
                        ?? Environment.GetEnvironmentVariable("FRONTEND_ORIGIN")
                        ?? "https://chatapp-gold-omega.vercel.app"; // default

var allowedOrigins = frontendOriginCsv
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(o => o.Trim())
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendOnly",
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure database exists and apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors("FrontendOnly"); // 🔹 Only allow frontend origin
app.MapControllers();    // 🔹 Controller route’larını ekle

app.Run();
