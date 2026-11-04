namespace RealtorApi.Infrastructure.Results;

public sealed record ResultError(
    string Code,
    string Title,
    int Status,
    string? Detail = null,
    IReadOnlyDictionary<string, object?>? Extensions = null);

public class Result
{
    private Result(bool isSuccess, ResultError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ResultError? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(ResultError error) => new(false, error);
}

public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, ResultError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public ResultError? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(ResultError error) => new(false, default, error);
}
