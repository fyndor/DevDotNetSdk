namespace DevDotNetSdk.Models;

public readonly struct Result<TValue, TError>
{
    private TValue? Value { get; }
    private TError? Error { get; }

    public Result()
    {
        Value = default;
        Error = default;
    }

    private Result(TValue? value, TError? error)
    {
        Value = value;
        Error = error;
    }

    public static Result<TValue, TError> Ok(TValue value) => new(value, default);

    public static Result<TValue, TError> Fail(TError error) => new(default, error);

    public readonly bool IsOk => Error is null;

    public readonly bool IsError => Error is not null;

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
    }

    private Result(TError? error)
    {
        Error = error;
    }

    public static Result<TError> Ok() => new(default);

    public static Result<TError> Fail(TError error) => new(error);

    public readonly bool IsOk => Error is null;

    public readonly bool IsError => Error is not null;

    public readonly TError UnwrapError() => Error ?? throw new InvalidOperationException("Cannot unwrap an error Result.");
}