using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Platform.Logging.MongoDb;

public sealed class LogDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Platform { get; set;  }

    [BsonRepresentation(BsonType.String)]
    public LogType LogType { get; set; }
    public string Action { get; set; } = null!;
    public string Source { get; set; } = null!;
    public DateTime AtUtc { get; set; }
    public BsonDocument? Payload { get; set; }
}