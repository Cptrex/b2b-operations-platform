namespace Platform.Logging.MongoDb;

public class MongoDbOptions
{
    public string ConnectionString { get; init; } = null!;
    public string Database { get; init; } = null!;
    public string Collection { get; init; } = "logs";
}
