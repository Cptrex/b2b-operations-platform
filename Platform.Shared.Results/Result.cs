namespace Platform.Shared.Results;

public sealed class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    private Result()
    {
        IsSuccess = true;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result Ok()
    {
        return new Result();
    }

    public static Result Fail(Error error)
    {
        return new Result(error);
    }
}
