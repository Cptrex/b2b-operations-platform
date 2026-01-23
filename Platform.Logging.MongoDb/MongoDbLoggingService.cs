using MongoDB.Bson;
using MongoDB.Driver;
using Platform.Identity.Abstractions.Contracts;
using Platform.Logging.MongoDb.Contracts;
using System.Reflection;

namespace Platform.Logging.MongoDb;

public class MongoDbLoggingService : ILoggingService
{
    private readonly IMongoDatabase _database;
    private readonly IActorProvider _actorPrivder;

    public MongoDbLoggingService(IMongoDatabase database, IActorProvider actorPrivder)
    {
        _database = database;
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
            Payload = payload.ToBsonDocument(),
            AtUtc = DateTime.UtcNow
        };

        var collectionName = type switch
        {
            LogType.Security => "audit_security",
            LogType.Activity => "auth_activity",
            LogType.Technical => "audit_technical",
            _ => "logs"
        };

        var collection = _database.GetCollection<LogDocument>(collectionName);

        await collection.InsertOneAsync(doc, cancellationToken: ct);
    }
}