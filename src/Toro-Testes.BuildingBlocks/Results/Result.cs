namespace Toro.Testes.BuildingBlocks.Results;

public class Result
{
    protected Result(bool isSuccess, string message, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public string Message { get; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public static Result Success(string message = "Success.") => new(true, message);

    public static Result Failure(string message, IReadOnlyDictionary<string, string[]>? errors = null) => new(false, message, errors);
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, string message, T? value, IReadOnlyDictionary<string, string[]>? errors = null)
        : base(isSuccess, message, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value, string message = "Success.") => new(true, message, value);

    public static new Result<T> Failure(string message, IReadOnlyDictionary<string, string[]>? errors = null)
        => new(false, message, default, errors);
}
