namespace Platform.Logging.MongoDb.Contracts;

public interface ILoggingService
{
    Task WriteAsync<Tpayload>(string action, string source, Tpayload? payload, CancellationToken ct = default);
}