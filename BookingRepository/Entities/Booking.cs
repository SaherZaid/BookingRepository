using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BookingRepository.Entities;

public class Booking
{
    [BsonRepresentation(BsonType.ObjectId), BsonId]
    public string? Id { get; set; }
    public int RoomId { get; set; } // the room booked
    public int CustomerId { get; set; } // the customer who booked
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}