using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelApp.Api.Models;

public sealed class Reservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string TableId { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime ReservationTime { get; set; }
    public int NumberOfGuests { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
