using HotelApp.Api.Helpers;
using HotelApp.Api.Interfaces;
using HotelApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(ApiResponse<List<TableEntity>>.Ok(await _tableService.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var table = await _tableService.GetByIdAsync(id);
        return table is null
            ? NotFound(ApiResponse<string>.Fail("Table not found."))
            : Ok(ApiResponse<TableEntity>.Ok(table));
    }

    [HttpPost]
    [Route("AddTable")]
    public async Task<IActionResult> Create([FromBody] TableEntity table) =>
        Ok(ApiResponse<TableEntity>.Ok(await _tableService.CreateAsync(table), "Table created successfully."));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromQuery] string status)
    {
        var updated = await _tableService.UpdateStatusAsync(id, status);
        return updated
            ? Ok(ApiResponse<string>.Ok("Updated", "Table status updated."))
            : NotFound(ApiResponse<string>.Fail("Table not found."));
    }
}
