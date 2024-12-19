using BookingRepository.DbContext;
using BookingRepository.Repository;
using BookingRepository.Repository;
using BookingRepository.DbContext;
using BookingRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IBookingRepository, BookingRepository.Repository.BookingRepository>();
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

var app = builder.Build();

app.MapGet("/bookings", async (IBookingRepository repo) =>
{
    return await repo.GetAllAsync();
});

app.Run("http://0.0.0.0:6050");