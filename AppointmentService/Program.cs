using AppointmentService.Application.Commands.CancelAppointment;
using AppointmentService.Application.Commands.CreateAppointment;
using AppointmentService.Application.Commands.RescheduleAppointment;
using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Interfaces.Messaging;
using AppointmentService.Application.Queries.GetAvailableSlots;
using AppointmentService.Infrastructure.Caching;
using AppointmentService.Infrastructure.Logging;
using AppointmentService.Infrastructure.Persistence;
using AppointmentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

//builder.Services.AddSingleton<IMessagePublisher>(
//    new RabbitMqPublisher("localhost"));
builder.Services.AddSingleton<IMessagePublisher>(provider =>
{
    return new RabbitMqPublisher("localhost");
});

builder.Services.AddScoped<CreateAppointmentHandler>(provider =>
{
    var repo = provider.GetRequiredService<IAppointmentRepository>();
    var cache = provider.GetRequiredService<ICacheService>();
    var publisher = provider.GetRequiredService<IMessagePublisher>();
    var userServiceClient = provider.GetRequiredService<IUserServiceClient>();
    var logService = provider.GetRequiredService<ILogService>(); 

    return new CreateAppointmentHandler(repo, cache, publisher, userServiceClient, logService);
});


builder.Services.AddScoped<GetAvailableSlotsHandler>(provider =>
{
    var repo = provider.GetRequiredService<IAppointmentRepository>();
    var cache = provider.GetRequiredService<ICacheService>();
    return new GetAvailableSlotsHandler(repo, cache);
});

builder.Services.AddScoped<CancelAppointmentHandler>(provider =>
{
    var repo = provider.GetRequiredService<IAppointmentRepository>();
    var cache = provider.GetRequiredService<ICacheService>();
    var publisher = provider.GetRequiredService<IMessagePublisher>();

    return new CancelAppointmentHandler(repo, cache, publisher);
});

builder.Services.AddScoped<RescheduleAppointmentHandler>(provider =>
{
    var repo = provider.GetRequiredService<IAppointmentRepository>();
    var cache = provider.GetRequiredService<ICacheService>();
    var publisher = provider.GetRequiredService<IMessagePublisher>();

    return new RescheduleAppointmentHandler(repo, cache, publisher);
});

builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7040/");
});


builder.Services.AddScoped<RescheduleAppointmentHandler>(provider =>
{
    var repo = provider.GetRequiredService<IAppointmentRepository>();
    var cache = provider.GetRequiredService<ICacheService>();
    var publisher = provider.GetRequiredService<IMessagePublisher>();

    return new RescheduleAppointmentHandler(repo, cache, publisher);
});



builder.Services.AddSingleton<ILogService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var section = config.GetSection("MongoDb");
    var connection = section["ConnectionString"];
    var database = section["Database"];
    return new MongoLogService(connection!, database!);
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
