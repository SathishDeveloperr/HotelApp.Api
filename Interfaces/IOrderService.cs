using HotelApp.Api.DTOs;
using HotelApp.Api.Models;

namespace HotelApp.Api.Interfaces;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(string id);
    Task<Order> CreateAsync(OrderDto dto);
    Task<bool> UpdateStatusAsync(string id, string status);
}
