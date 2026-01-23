using System.Text.Json;
using System.Text.Json.Serialization;

namespace Platform.Shared.Results;

public sealed class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public Error? Error { get; init; }

    [JsonConstructor]
    public Result(bool isSuccess, T? value, Error? error)
    {
        if (isSuccess)
        {
            if (value is null)
            {
                throw new JsonException("Success result must have value.");
            }
            if (error is not null)
            {
                throw new JsonException("Success result must not have error.");
            }
        }
        else
        {
            if (error is null)
            {
                throw new JsonException("Failed result must have error.");
            }
            if (value is not null)
            {
                throw new JsonException("Failed result must not have value.");
            }
        }

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value)
    {
        return new(true, value, null);
    }
    public static Result<T> Fail(Error error)
    {
        return new(false, default, error);
    }
}