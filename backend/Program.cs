using ChatBackend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // 🔹 Controller ekle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use connection string from env/config if provided, else default to chat.db
var connString = builder.Configuration.GetConnectionString("Default")
                 ?? builder.Configuration["ConnectionStrings:Default"]
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                 ?? "Data Source=chat.db";

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

app.UseHttpsRedirection();
app.UseCors("FrontendOnly"); // 🔹 Only allow frontend origin
app.MapControllers();    // 🔹 Controller route’larını ekle

app.Run();
