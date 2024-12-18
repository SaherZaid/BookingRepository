using BookingRepository.Entities;
using MongoDB.Driver;

namespace BookingRepository.Repository;

public class BookingRepository : IBookingRepository
{
    private readonly IMongoCollection<Booking> _collection;

    public BookingRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Booking>("Bookings");
    }

    public async Task<Booking> GetByIdAsync(string id)
    {
        return await _collection.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task AddAsync(Booking booking)
    {
        await _collection.InsertOneAsync(booking);
    }

    public async Task UpdateAsync(int id, Booking booking)
    {
        await _collection.ReplaceOneAsync(b => b.Id == booking.Id, booking);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(b => b.Id == id);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        var count = await _collection.CountDocumentsAsync(b =>
            b.RoomId == roomId &&
            ((b.CheckInDate < checkOutDate) && (b.CheckOutDate > checkInDate)));

        return count == 0;
    }
}