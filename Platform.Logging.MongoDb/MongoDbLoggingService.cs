using MongoDB.Driver;
using Platform.Identity.Abstractions.Contracts;
using Platform.Logging.MongoDb.Contracts;
using System.Reflection;

namespace Platform.Logging.MongoDb;

public class MongoDbLoggingService : ILoggingService
{
    private readonly IMongoCollection<LogDocument> _collection;
    private readonly IActorProvider _actorPrivder;
    public MongoDbLoggingService(IMongoCollection<LogDocument> collection, IActorProvider actorPrivder)
    {
        _collection = collection;
        _actorPrivder = actorPrivder;
    }

    public async Task WriteAsync<Tpayload>(LogType type, string action, Tpayload? payload, CancellationToken ct = default)
    {
        var doc = new LogDocument
        {
            LogType = type,
            Action = action,
            Platform = Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown",
            Source = _actorPrivder.GetCurrent()?.ActorId ?? "unknown",
            Payload = payload,
            AtUtc = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(doc, cancellationToken: ct);
    }
}