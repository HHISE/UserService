namespace UserService.Repositories;

using MongoDB.Driver;
using Models;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IConfiguration configuration)
    {
        var connectionString = configuration["MONGODB_CONNECTION_STRING"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("UserDatabase");

        _users = database.GetCollection<User>("Users");
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateAsync(int id, User user)
    {
        await _users.ReplaceOneAsync(
            existingUser => existingUser.Id == id,
            user
        );
    }

    public async Task DeleteAsync(int id)
    {
        await _users.DeleteOneAsync(user => user.Id == id);
    }
}