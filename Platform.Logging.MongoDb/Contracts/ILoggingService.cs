namespace Platform.Logging.MongoDb.Contracts;

public interface ILoggingService
{
    Task WriteAsync<Tpayload>(LogType type, string action, Tpayload? payload, CancellationToken ct = default);
}