using MongoDB.Bson.Serialization.Attributes;

namespace HotelApp.Api.Models;

public sealed class OrderItem
{
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string MenuItemId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
