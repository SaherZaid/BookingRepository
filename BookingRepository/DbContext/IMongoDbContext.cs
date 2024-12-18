using MongoDB.Driver;

namespace BookingRepository.DbContext;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}