namespace DevDotNetSdk.Models;

public readonly struct Result<TValue, TError>
{
    private TValue? Value { get; }
    private TError? Error { get; }

    public Result()
    {
        Value = default;
        Error = default;
        IsOk = false;
    }

    private Result(TValue? value, TError? error, bool isOk)
    {
        Value = value;
        Error = error;
        IsOk = isOk;
    }

    public static Result<TValue, TError> Ok(TValue value) => new(value, default, true);

    public static Result<TValue, TError> Fail(TError error) => new(default, error, false);

    public readonly bool IsOk { get; }

    public readonly bool IsError => !IsOk;

    public readonly bool ValueIsNull => Value is null;
    
    public readonly TValue Unwrap() => Value ?? throw new InvalidOperationException("Cannot unwrap a Result with no value.");

    public readonly TError UnwrapError() => Error ?? throw new InvalidOperationException("Cannot unwrap an error Result.");
}

public readonly struct Result<TError>
{
    public TError? Error { get; }

    public Result()
    {
        Error = default;
        IsOk = true;
    }

    private Result(TError? error, bool isOk)
    {
        Error = error;
    }

    public static Result<TError> Ok() => new(default, true);

    public static Result<TError> Fail(TError error) => new(error, false);

    public readonly bool IsOk { get; }

    public readonly bool IsError => !IsOk;

    public readonly TError UnwrapError() => Error ?? throw new InvalidOperationException("Cannot unwrap an error Result.");
}