using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MVCWebApp.Models
{
    public class VisitorEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? IpAddress { get; set; }
        public DateTime VisitTime { get; set; }
    }
}
