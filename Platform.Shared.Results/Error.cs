using Platform.Shared.Results.Enums;

namespace Platform.Shared.Results;

public class Error
{
    public string Message { get; }
    public ResultErrorCategory Category { get; }
    public Error(string message, ResultErrorCategory category)
    {
        Message = message;
        Category = category;
    }
}