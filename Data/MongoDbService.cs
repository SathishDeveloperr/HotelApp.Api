using HotelApp.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HotelApp.Api.Data;

public sealed class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<TableEntity> Tables => _database.GetCollection<TableEntity>("Tables");
    public IMongoCollection<Reservation> Reservations => _database.GetCollection<Reservation>("Reservations");
    public IMongoCollection<MenuItem> MenuItems => _database.GetCollection<MenuItem>("MenuItems");
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
    public IMongoCollection<Payment> Payments => _database.GetCollection<Payment>("Payments");
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
}
