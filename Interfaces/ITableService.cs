using HotelApp.Api.Models;

namespace HotelApp.Api.Interfaces;

public interface ITableService
{
    Task<List<TableEntity>> GetAllAsync();
    Task<TableEntity?> GetByIdAsync(string id);
    Task<TableEntity> CreateAsync(TableEntity table);
    Task<bool> UpdateStatusAsync(string id, string status);
}
