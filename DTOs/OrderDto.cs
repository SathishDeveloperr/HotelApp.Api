namespace HotelApp.Api.DTOs;

public sealed class OrderDto
{
    public string TableId { get; set; } = string.Empty;
    public string? ReservationId { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}

public sealed class OrderItemDto
{
    public string MenuItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
