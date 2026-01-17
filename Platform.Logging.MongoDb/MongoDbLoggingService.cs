using MongoDB.Driver;
using Platform.Logging.MongoDb.Contracts;

namespace Platform.Logging.MongoDb;

public class MongoDbLoggingService : ILoggingService
{
    private readonly IMongoCollection<LogDocument> _collection;

    public MongoDbLoggingService(IMongoCollection<LogDocument> collection)
    {
        _collection = collection;
    }

    public async Task WriteAsync<Tpayload>(string action, string source, Tpayload? payload, CancellationToken ct = default)
    {
        var doc = new LogDocument
        {
            Action = action,
            Source = source,
            Payload = payload,
            AtUtc = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(doc, cancellationToken: ct);
    }
}