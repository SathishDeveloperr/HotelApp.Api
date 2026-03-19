using HotelApp.Api.Helpers;
using HotelApp.Api.Models;
using HotelApp.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Api.Services;

namespace HotelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ITableService _tableService;
    private readonly MenuService _menuService;

    public DashboardController(
        IOrderService orderService,
        ITableService tableService,
        MenuService menuService)
    {
        _orderService = orderService;
        _tableService = tableService;
        _menuService = menuService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var orders = await _orderService.GetAllAsync();
        var tables = await _tableService.GetAllAsync();

        var summary = new
        {
            totalOrders = orders.Count,
            totalRevenue = orders.Where(o => o.Status == "Paid").Sum(o => o.TotalAmount),
            completedOrders = orders.Count(o => o.Status == "Completed" || o.Status == "Paid"),
            pendingOrders = orders.Count(o => o.Status == "Placed" || o.Status == "Preparing"),
            occupiedTables = tables.Count(t => t.Status == "Occupied"),
            availableTables = tables.Count(t => t.Status == "Available"),
            reservedTables = tables.Count(t => t.Status == "Reserved")
        };

        return Ok(ApiResponse<object>.Ok(summary, "Dashboard summary retrieved successfully."));
    }

    [HttpGet("orders-by-table")]
    public async Task<IActionResult> GetOrdersByTable()
    {
        var orders = await _orderService.GetAllAsync();

        var ordersByTable = orders
            .GroupBy(o => o.TableId)
            .Select(g => new
            {
                tableId = g.Key,
                totalOrders = g.Count(),
                completedOrders = g.Count(o => o.Status == "Completed" || o.Status == "Paid"),
                totalRevenue = g.Where(o => o.Status == "Paid").Sum(o => o.TotalAmount),
                averageOrderValue = g.Where(o => o.Status == "Paid").Any()
                    ? g.Where(o => o.Status == "Paid").Average(o => o.TotalAmount)
                    : 0
            })
            .OrderByDescending(x => x.totalOrders)
            .ToList();

        return Ok(ApiResponse<object>.Ok(ordersByTable, "Orders by table retrieved successfully."));
    }

    [HttpGet("top-foods")]
    public async Task<IActionResult> GetTopFoods(int limit = 10)
    {
        var orders = await _orderService.GetAllAsync();

        var topFoods = orders
            .Where(o => o.Items != null && o.Items.Count > 0)
            .SelectMany(o => o.Items)
            .GroupBy(i => i.MenuItemId)
            .Select(g => new
            {
                menuItemId = g.Key,
                totalQuantity = g.Sum(i => i.Quantity),
                totalRevenue = g.Sum(i => i.TotalPrice),
                orderCount = g.Count()
            })
            .OrderByDescending(x => x.totalQuantity)
            .Take(limit)
            .ToList();

        return Ok(ApiResponse<object>.Ok(topFoods, "Top foods retrieved successfully."));
    }

    [HttpGet("top-foods-details")]
    public async Task<IActionResult> GetTopFoodsWithDetails(int limit = 10)
    {
        var orders = await _orderService.GetAllAsync();
        var menuItems = await _menuService.GetAllAsync();

        var topFoods = orders
            .Where(o => o.Items != null && o.Items.Count > 0)
            .SelectMany(o => o.Items)
            .GroupBy(i => i.MenuItemId)
            .Select(g => new
            {
                menuItemId = g.Key,
                menuItemName = g.FirstOrDefault()?.Name ?? "Unknown",
                category = menuItems.FirstOrDefault(m => m.Id == g.Key)?.Category ?? "Unknown",
                unitPrice = menuItems.FirstOrDefault(m => m.Id == g.Key)?.Price ?? 0,
                totalQuantity = g.Sum(i => i.Quantity),
                totalRevenue = g.Sum(i => i.TotalPrice),
                orderCount = g.Count(),
                averageUnitPrice = g.Average(i => i.UnitPrice)
            })
            .OrderByDescending(x => x.totalQuantity)
            .Take(limit)
            .ToList();

        return Ok(ApiResponse<object>.Ok(topFoods, "Top foods with details retrieved successfully."));
    }

    [HttpGet("sales-by-category")]
    public async Task<IActionResult> GetSalesByCategory()
    {
        var orders = await _orderService.GetAllAsync();
        var menuItems = await _menuService.GetAllAsync();

        var salesByCategory = orders
            .Where(o => o.Items != null && o.Items.Count > 0)
            .SelectMany(o => o.Items)
            .GroupBy(i => menuItems.FirstOrDefault(m => m.Id == i.MenuItemId)?.Category ?? "Unknown")
            .Select(g => new
            {
                category = g.Key,
                totalItems = g.Sum(i => i.Quantity),
                totalRevenue = g.Sum(i => i.TotalPrice),
                orderCount = g.Count(),
                averageItemPrice = g.Average(i => i.UnitPrice)
            })
            .OrderByDescending(x => x.totalRevenue)
            .ToList();

        return Ok(ApiResponse<object>.Ok(salesByCategory, "Sales by category retrieved successfully."));
    }

    [HttpGet("order-status-distribution")]
    public async Task<IActionResult> GetOrderStatusDistribution()
    {
        var orders = await _orderService.GetAllAsync();

        if (!orders.Any())
            return Ok(ApiResponse<object>.Ok("", "Order status distribution retrieved successfully."));

        var statusDistribution = orders
            .GroupBy(o => o.Status)
            .Select(g => new
            {
                status = g.Key,
                count = g.Count(),
                percentage = (decimal)g.Count() / orders.Count * 100,
                revenue = g.Sum(o => o.TotalAmount)
            })
            .OrderByDescending(x => x.count)
            .ToList();

        return Ok(ApiResponse<object>.Ok(statusDistribution, "Order status distribution retrieved successfully."));
    }

    [HttpGet("table-occupancy")]
    public async Task<IActionResult> GetTableOccupancy()
    {
        var tables = await _tableService.GetAllAsync();

        var occupancy = new
        {
            total = tables.Count,
            occupied = tables.Count(t => t.Status == "Occupied"),
            available = tables.Count(t => t.Status == "Available"),
            reserved = tables.Count(t => t.Status == "Reserved"),
            occupancyRate = tables.Any()
                ? ((decimal)tables.Count(t => t.Status == "Occupied") / tables.Count * 100)
                : 0,
            statusBreakdown = tables
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count()
                })
                .ToList()
        };

        return Ok(ApiResponse<object>.Ok(occupancy, "Table occupancy retrieved successfully."));
    }

    [HttpGet("daily-revenue")]
    public async Task<IActionResult> GetDailyRevenue(int days = 30)
    {
        var orders = await _orderService.GetAllAsync();
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        var dailyRevenue = orders
            .Where(o => o.OrderedAt >= cutoffDate && (o.Status == "Completed" || o.Status == "Paid"))
            .GroupBy(o => o.OrderedAt.Date)
            .Select(g => new
            {
                date = g.Key.ToString("yyyy-MM-dd"),
                revenue = g.Sum(o => o.TotalAmount),
                orderCount = g.Count(),
                averageOrderValue = g.Average(o => o.TotalAmount)
            })
            .OrderBy(x => x.date)
            .ToList();

        return Ok(ApiResponse<object>.Ok(dailyRevenue, "Daily revenue retrieved successfully."));
    }

    [HttpGet("peak-hours")]
    public async Task<IActionResult> GetPeakHours()
    {
        var orders = await _orderService.GetAllAsync();

        if (!orders.Any())
            return Ok(ApiResponse<object>.Ok("", "Peak hours retrieved successfully."));

        var peakHours = orders
            .GroupBy(o => o.OrderedAt.Hour)
            .Select(g => new
            {
                hour = g.Key,
                hourRange = $"{g.Key:D2}:00 - {g.Key:D2}:59",
                orderCount = g.Count(),
                totalRevenue = g.Sum(o => o.TotalAmount),
                averageOrderValue = g.Average(o => o.TotalAmount)
            })
            .OrderBy(x => x.hour)
            .ToList();

        return Ok(ApiResponse<object>.Ok(peakHours, "Peak hours retrieved successfully."));
    }
}