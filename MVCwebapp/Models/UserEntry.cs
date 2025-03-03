using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MVCWebApp.Models
{
    public class UserEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;
    }
}

