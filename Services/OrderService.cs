using HotelApp.Api.Constants;
using HotelApp.Api.Data;
using HotelApp.Api.DTOs;
using HotelApp.Api.Interfaces;
using HotelApp.Api.Models;
using MongoDB.Driver;

namespace HotelApp.Api.Services;

public sealed class OrderService : IOrderService
{
    private readonly MongoDbService _mongoDbService;

    public OrderService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public async Task<List<Order>> GetAllAsync() =>
        await _mongoDbService.Orders.Find(_ => true).SortByDescending(x => x.OrderedAt).ToListAsync();

    public async Task<Order?> GetByIdAsync(string id) =>
        await _mongoDbService.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Order> CreateAsync(OrderDto dto)
    {
        var table = await _mongoDbService.Tables.Find(t => t.Id == dto.TableId).FirstOrDefaultAsync();
        if (table is null)
        {
            throw new InvalidOperationException("Table not found.");
        }

        if (dto.Items.Count == 0)
        {
            throw new InvalidOperationException("Order must contain at least one item.");
        }

        var menuIds = dto.Items.Select(x => x.MenuItemId).ToList();
        var menuItems = await _mongoDbService.MenuItems.Find(x => menuIds.Contains(x.Id) && x.IsAvailable).ToListAsync();
        var menuLookup = menuItems.ToDictionary(x => x.Id, x => x);

        var orderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            if (!menuLookup.TryGetValue(item.MenuItemId, out var menuItem))
            {
                throw new InvalidOperationException($"Menu item not found or unavailable: {item.MenuItemId}");
            }

            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException("Item quantity must be greater than zero.");
            }

            orderItems.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                Name = menuItem.Name,
                Quantity = item.Quantity,
                UnitPrice = menuItem.Price
            });
        }

        var order = new Order
        {
            TableId = dto.TableId,
            ReservationId = dto.ReservationId,
            Items = orderItems,
            TotalAmount = orderItems.Sum(x => x.TotalPrice),
            Status = AppConstants.OrderPlaced
        };

        await _mongoDbService.Orders.InsertOneAsync(order);

        await _mongoDbService.Tables.UpdateOneAsync(
            t => t.Id == dto.TableId,
            Builders<TableEntity>.Update.Set(t => t.Status, AppConstants.TableOccupied));

        return order;
    }

    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var updateResult = await _mongoDbService.Orders.UpdateOneAsync(
            x => x.Id == id,
            Builders<Order>.Update.Set(x => x.Status, status));

        return updateResult.ModifiedCount > 0;
    }
}
