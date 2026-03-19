using HotelApp.Api.Helpers;
using HotelApp.Api.Models;
using HotelApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(ApiResponse<List<Payment>>.Ok(await _paymentService.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        return payment is null
            ? NotFound(ApiResponse<string>.Fail("Payment not found."))
            : Ok(ApiResponse<Payment>.Ok(payment));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Payment payment) =>
        Ok(ApiResponse<Payment>.Ok(await _paymentService.CreateAsync(payment), "Payment processed successfully."));
}
