using System.Security.Cryptography;
using BookingRepository.Entities;

namespace BookingRepository.Repository;

public interface IBookingRepository
{
    Task<Booking> GetByIdAsync(string id);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task AddAsync(Booking entity);
    Task UpdateAsync(int id, Booking booking);
    Task DeleteAsync(string id);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkInDate, DateTime checkOutDate);
}