using HotelApp.Api.DTOs;
using HotelApp.Api.Helpers;
using HotelApp.Api.Interfaces;
using HotelApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(ApiResponse<List<Reservation>>.Ok(await _reservationService.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        return reservation is null
            ? NotFound(ApiResponse<string>.Fail("Reservation not found."))
            : Ok(ApiResponse<Reservation>.Ok(reservation));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservationDto dto) =>
        Ok(ApiResponse<Reservation>.Ok(await _reservationService.CreateAsync(dto), "Reservation created successfully."));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromQuery] string status)
    {
        var updated = await _reservationService.UpdateStatusAsync(id, status);
        return updated
            ? Ok(ApiResponse<string>.Ok("Updated", "Reservation status updated."))
            : NotFound(ApiResponse<string>.Fail("Reservation not found."));
    }
}
