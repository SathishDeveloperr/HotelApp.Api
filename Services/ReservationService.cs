using HotelApp.Api.Constants;
using HotelApp.Api.Data;
using HotelApp.Api.DTOs;
using HotelApp.Api.Interfaces;
using HotelApp.Api.Models;
using MongoDB.Driver;

namespace HotelApp.Api.Services;

public sealed class ReservationService : IReservationService
{
    private readonly MongoDbService _mongoDbService;

    public ReservationService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    public async Task<List<Reservation>> GetAllAsync() =>
        await _mongoDbService.Reservations.Find(_ => true).SortByDescending(x => x.ReservationTime).ToListAsync();

    public async Task<Reservation?> GetByIdAsync(string id) =>
        await _mongoDbService.Reservations.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Reservation> CreateAsync(ReservationDto dto)
    {
        var table = await _mongoDbService.Tables.Find(t => t.Id == dto.TableId).FirstOrDefaultAsync();
        if (table is null)
        {
            throw new InvalidOperationException("Table not found.");
        }

        if (dto.NumberOfGuests > table.Capacity)
        {
            throw new InvalidOperationException("Guest count exceeds table capacity.");
        }

        var reservation = new Reservation
        {
            TableId = dto.TableId,
            CustomerName = dto.CustomerName,
            PhoneNumber = dto.PhoneNumber,
            ReservationTime = dto.ReservationTime,
            NumberOfGuests = dto.NumberOfGuests,
            Notes = dto.Notes,
            Status = AppConstants.ReservationPending
        };

        await _mongoDbService.Reservations.InsertOneAsync(reservation);

        await _mongoDbService.Tables.UpdateOneAsync(
            t => t.Id == dto.TableId,
            Builders<TableEntity>.Update.Set(t => t.Status, AppConstants.TableReserved));

        return reservation;
    }

    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var updateResult = await _mongoDbService.Reservations.UpdateOneAsync(
            x => x.Id == id,
            Builders<Reservation>.Update.Set(x => x.Status, status));

        return updateResult.ModifiedCount > 0;
    }
}
