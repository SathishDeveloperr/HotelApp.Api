namespace HotelApp.Api.DTOs;

public sealed class ReservationDto
{
    public string TableId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime ReservationTime { get; set; }
    public int NumberOfGuests { get; set; }
    public string? Notes { get; set; }
}
