using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelApp.Api.Models;

public sealed class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string TableId { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ReservationId { get; set; }

    public List<OrderItem> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
}
