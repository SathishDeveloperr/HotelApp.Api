using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelApp.Api.Models;

public sealed class TableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
}
