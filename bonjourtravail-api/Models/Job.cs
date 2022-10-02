using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace bonjourtravail_api.Models;

public class Job : Offre
{
    public bool Internal { get; set; } = false;
}