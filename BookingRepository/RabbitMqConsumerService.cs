using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using BookingRepository;
using BookingRepository.Repository;
using BookingRepository.Entities;
using BookingRepository.Repository;
using MongoDB.Driver;
using RabbitMQ.Client.Events;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly RabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    public RabbitMqConsumerService(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, ILogger<RabbitMqConsumerService> logger)
    {
        _rabbitMqService = rabbitMqService;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_rabbitMqService.Channel);
        consumer.Received += async (model, args) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received RabbitMQ message: {Message}", message);

                var booking = JsonSerializer.Deserialize<Booking>(message);
                if (booking == null)
                {
                    throw new InvalidOperationException("Deserialized booking is null or invalid.");
                }

                await repository.AddAsync(booking);
                _logger.LogInformation("Booking successfully processed and saved: {BookingId}", booking.Id);

                _rabbitMqService.Acknowledge(args.DeliveryTag);
                _logger.LogInformation("Message acknowledged to RabbitMQ.");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error while processing message: {Message}", Encoding.UTF8.GetString(args.Body.ToArray()));
            }
            catch (MongoException mongoEx)
            {
                _logger.LogError(mongoEx, "MongoDB operation failed while saving the booking.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing RabbitMQ message.");
            }
        };

        _rabbitMqService.Consume(consumer);
        return Task.CompletedTask;
    }
}