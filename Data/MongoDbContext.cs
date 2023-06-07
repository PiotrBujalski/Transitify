using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Transitify.Models;

namespace Transitify.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            var databaseName = configuration.GetValue<string>("DatabaseSettings:DatabaseName");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Ticket> Tickets => _database.GetCollection<Ticket>("Tickets");
    }
}