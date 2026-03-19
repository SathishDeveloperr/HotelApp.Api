using HotelApp.Api.Constants;
using HotelApp.Api.Data;
using HotelApp.Api.Interfaces;
using HotelApp.Api.Models;
using MongoDB.Driver;

namespace HotelApp.Api.Services;

public sealed class TableService : ITableService
{
    private readonly MongoDbService _mongoDbService;

    public TableService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public async Task<List<TableEntity>> GetAllAsync() =>
        await _mongoDbService.Tables.Find(_ => true).SortBy(x => x.TableNumber).ToListAsync();

    public async Task<TableEntity?> GetByIdAsync(string id) =>
        await _mongoDbService.Tables.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<TableEntity> CreateAsync(TableEntity table)
    {
        table.Status = string.IsNullOrWhiteSpace(table.Status) ? AppConstants.TableAvailable : table.Status;
        await _mongoDbService.Tables.InsertOneAsync(table);
        return table;
    }

    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var updateResult = await _mongoDbService.Tables.UpdateOneAsync(
            x => x.Id == id,
            Builders<TableEntity>.Update.Set(x => x.Status, status));

        return updateResult.ModifiedCount > 0;
    }
}
