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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // 🔹 CORS aktif et
app.MapControllers();    // 🔹 Controller route’larını ekle

app.Run();
