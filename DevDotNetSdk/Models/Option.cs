namespace DevDotNetSdk.Models;

public readonly struct Option<TValue>
{
    private TValue? Value { get; }
    private bool IsSet { get; }

    private Option(TValue value)
    {
        Value = value;
        IsSet = true;
    }

    public Option()
    {
        Value = default;
        IsSet = false;
    }

    public static Option<TValue> FromSome(TValue value) => new(value);

    public static Option<TValue> FromNone() => new();

    public readonly bool IsSome => IsSet && Value is not null;

    public readonly bool IsNone => !IsSet;

    public readonly TValue Unwrap() => Value ?? throw new InvalidOperationException("Cannot unwrap an Option with no value.");
}