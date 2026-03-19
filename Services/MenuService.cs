using HotelApp.Api.Data;
using HotelApp.Api.Models;
using MongoDB.Driver;

namespace HotelApp.Api.Services;

public sealed class MenuService
{
    private readonly MongoDbService _mongoDbService;

    public MenuService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public async Task<List<MenuItem>> GetAllAsync() =>
        await _mongoDbService.MenuItems.Find(_ => true).SortBy(x => x.Name).ToListAsync();

    public async Task<MenuItem?> GetByIdAsync(string id) =>
        await _mongoDbService.MenuItems.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<MenuItem> CreateAsync(MenuItem item)
    {
        await _mongoDbService.MenuItems.InsertOneAsync(item);
        return item;
    }

    public async Task<bool> UpdateAsync(string id, MenuItem item)
    {
        item.Id = id;
        var updateResult = await _mongoDbService.MenuItems.ReplaceOneAsync(x => x.Id == id, item);
        return updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var deleteResult = await _mongoDbService.MenuItems.DeleteOneAsync(x => x.Id == id);
        return deleteResult.DeletedCount > 0;
    }
}
