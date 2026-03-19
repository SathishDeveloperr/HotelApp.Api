using HotelApp.Api.DTOs;
using HotelApp.Api.Models;

namespace HotelApp.Api.Interfaces;

public interface IReservationService
{
    Task<List<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(string id);
    Task<Reservation> CreateAsync(ReservationDto dto);
    Task<bool> UpdateStatusAsync(string id, string status);
}
