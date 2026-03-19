using HotelApp.Api.DTOs;
using HotelApp.Api.Helpers;
using HotelApp.Api.Interfaces;
using HotelApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(ApiResponse<List<Order>>.Ok(await _orderService.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return order is null
            ? NotFound(ApiResponse<string>.Fail("Order not found."))
            : Ok(ApiResponse<Order>.Ok(order));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderDto dto) =>
        Ok(ApiResponse<Order>.Ok(await _orderService.CreateAsync(dto), "Order created successfully."));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromQuery] string status)
    {
        var updated = await _orderService.UpdateStatusAsync(id, status);
        return updated
            ? Ok(ApiResponse<string>.Ok("Updated", "Order status updated."))
            : NotFound(ApiResponse<string>.Fail("Order not found."));
    }
}
