namespace BookingRepository.Entities;

public class Receiver
{
    public string ExchangeName { get; set; }
    public string RoutingKey { get; set; }
    public string QueueName { get; set; }
}