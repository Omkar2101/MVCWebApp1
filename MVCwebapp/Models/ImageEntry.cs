using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MVCWebApp.Models
{
    public class ImageEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string FileName { get; set; } = null!;

        public string Text { get; set; } = null!;

        public string FileId { get; set; } = null!; // GridFS File ID
    }
}
