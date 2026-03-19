using HotelApp.Api.Constants;
using HotelApp.Api.Data;
using HotelApp.Api.Models;
using MongoDB.Driver;

namespace HotelApp.Api.Services;

public sealed class PaymentService
{
    private readonly MongoDbService _mongoDbService;

    public PaymentService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public async Task<List<Payment>> GetAllAsync() =>
        await _mongoDbService.Payments.Find(_ => true).SortByDescending(x => x.PaidAt).ToListAsync();

    public async Task<Payment?> GetByIdAsync(string id) =>
        await _mongoDbService.Payments.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Payment> CreateAsync(Payment payment)
    {
        var order = await _mongoDbService.Orders.Find(x => x.Id == payment.OrderId).FirstOrDefaultAsync();
        if (order is null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        payment.Amount = payment.Amount <= 0 ? order.TotalAmount : payment.Amount;
        payment.Status = string.IsNullOrWhiteSpace(payment.Status) ? AppConstants.PaymentCompleted : payment.Status;
        payment.PaidAt = DateTime.UtcNow;

        await _mongoDbService.Payments.InsertOneAsync(payment);

        await _mongoDbService.Orders.UpdateOneAsync(
            x => x.Id == payment.OrderId,
            Builders<Order>.Update.Set(x => x.Status, AppConstants.OrderPaid));

        return payment;
    }
}
