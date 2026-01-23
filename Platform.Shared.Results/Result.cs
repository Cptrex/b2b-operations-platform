using System.Text.Json;
using System.Text.Json.Serialization;

namespace Platform.Shared.Results;

public sealed class Result
{
    public bool IsSuccess { get; init; }
    public Error? Error { get; init; }

    [JsonConstructor]
    public Result(bool isSuccess, Error? error)
    {
        if (isSuccess)
        {
            if (error is not null)
                throw new JsonException("Success result must not have error.");
        }
        else
        {
            if (error is null)
                throw new JsonException("Failed result must have error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok()
    {
        return new(true, null);
    }
    public static Result Fail(Error error)
    {
        return new(false, error);
    }
}