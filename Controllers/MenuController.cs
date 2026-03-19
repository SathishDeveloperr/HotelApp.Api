using HotelApp.Api.Helpers;
using HotelApp.Api.Models;
using HotelApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MenuController : ControllerBase
{
    private readonly MenuService _menuService;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(ApiResponse<List<MenuItem>>.Ok(await _menuService.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var item = await _menuService.GetByIdAsync(id);
        return item is null
            ? NotFound(ApiResponse<string>.Fail("Menu item not found."))
            : Ok(ApiResponse<MenuItem>.Ok(item));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MenuItem item) =>
        Ok(ApiResponse<MenuItem>.Ok(await _menuService.CreateAsync(item), "Menu item created successfully."));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] MenuItem item)
    {
        var updated = await _menuService.UpdateAsync(id, item);
        return updated
            ? Ok(ApiResponse<string>.Ok("Updated", "Menu item updated."))
            : NotFound(ApiResponse<string>.Fail("Menu item not found."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _menuService.DeleteAsync(id);
        return deleted
            ? Ok(ApiResponse<string>.Ok("Deleted", "Menu item deleted."))
            : NotFound(ApiResponse<string>.Fail("Menu item not found."));
    }
}
