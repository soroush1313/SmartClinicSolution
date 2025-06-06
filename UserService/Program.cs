using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UserService.Application.Interfaces;
using UserService.Infrastructure;
using UserService.Infrastructure.Services;
using UserService.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// ? 1. Load Configuration
// --------------------------

var configuration = builder.Configuration;

// ----------------------------
// ? 2. Add Services to Container
// ----------------------------

// SQL Server (EF Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Redis connection string is missing in configuration.");

    return ConnectionMultiplexer.Connect(redisConnectionString);
});


// Redis Cache Service
builder.Services.AddSingleton<RedisCacheService>();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing in configuration.");

builder.Services.AddSingleton(new JwtService(jwtKey));


// User Service
builder.Services.AddScoped<IUserService, UserService.Infrastructure.Services.UserService>();

// Controller & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ----------------------------
// ? 3. Build App
// ----------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Optional Middleware
app.UseHttpsRedirection();

app.UseAuthorization(); // ?? ??? ???? ?? ????? ???? ?????

app.MapControllers();

app.Run();


// Trigger GitHub Actions