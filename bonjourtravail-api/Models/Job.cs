using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bonjourtravail_api.Models;

public class Job
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string JobName { get; set; } = null!;

    public decimal ContractType { get; set; }

    public string Corpotation { get; set; } = null!;

    public string Country { get; set; } = null!;
}