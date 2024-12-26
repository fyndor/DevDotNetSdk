using System.Diagnostics.CodeAnalysis;

namespace DevDotNetSdk.Models;

public readonly struct Variant<T1, T2>
{
    private readonly byte tag;
    private readonly T1? valueT1;
    private readonly T2? valueT2;

    /// <summary>
    /// Constructor for T1.
    /// </summary>
    public Variant(T1 value)
    {
        tag     = 1;
        valueT1 = value;
        valueT2 = default;
    }

    /// <summary>
    /// Constructor for T2.
    /// </summary>
    public Variant(T2 value)
    {
        tag     = 2;
        valueT1 = default;
        valueT2 = value;
    }

    /// <summary>
    /// Checks if this Variant currently holds a value of type T.
    /// </summary>
    public bool Is<T>()
    {
        if (typeof(T) == typeof(T1) && tag == 1) return true;
        if (typeof(T) == typeof(T2) && tag == 2) return true;
        return false;
    }

    /// <summary>
    /// Get the stored value as T, or throw if mismatched.
    /// </summary>
    public T Get<T>()
    {
        if (!Is<T>()) 
        {
            throw new InvalidOperationException($"Variant does not hold {typeof(T).Name}.");
        }

        if (typeof(T) == typeof(T1) && valueT1 is not null)
        {
            return (T)(object)valueT1;
        }
        
        if (typeof(T) == typeof(T2) && valueT2 is not null)
        {
            return (T)(object)valueT2;
        }
        throw new InvalidOperationException("Variant value is null.");
    }

    /// <summary>
    /// Try to get the stored value as T. Returns true if successful.
    /// </summary>
    public bool TryGet<T>([NotNullWhen(true)] out T? value)
    {
        if (Is<T>())
        {
            value = Get<T>()!;
            return true;
        }
        value = default;
        return false;
    }
}

public readonly struct Variant<T1, T2, T3>
{
    private readonly byte tag;
    private readonly T1? valueT1;
    private readonly T2? valueT2;
    private readonly T3? valueT3;

    /// <summary>
    /// Constructor for T1.
    /// </summary>
    public Variant(T1 value)
    {
        tag     = 1;
        valueT1 = value;
        valueT2 = default;
        valueT3 = default;
    }

    /// <summary>
    /// Constructor for T2.
    /// </summary>
    public Variant(T2 value)
    {
        tag     = 2;
        valueT1 = default;
        valueT2 = value;
        valueT3 = default;
    }

    /// <summary>
    /// Constructor for T3.
    /// </summary>
    public Variant(T3 value)
    {
        tag     = 3;
        valueT1 = default;
        valueT2 = default;
        valueT3 = value;
    }

    /// <summary>
    /// Checks if this Variant currently holds a value of type T.
    /// </summary>
    public bool Is<T>()
    {
        if (typeof(T) == typeof(T1) && tag == 1) return true;
        if (typeof(T) == typeof(T2) && tag == 2) return true;
        if (typeof(T) == typeof(T3) && tag == 3) return true;
        return false;
    }

    /// <summary>
    /// Get the stored value as T, or throw if mismatched or null.
    /// </summary>
    public T Get<T>()
    {
        if (!Is<T>())
        {
            throw new InvalidOperationException(
                $"Variant does not hold {typeof(T).Name}."
            );
        }

        // T must be T1, T2, or T3, and the tag must match.
        if (typeof(T) == typeof(T1) && valueT1 is not null)
        {
            return (T)(object)valueT1;
        }
        if (typeof(T) == typeof(T2) && valueT2 is not null)
        {
            return (T)(object)valueT2;
        }
        if (typeof(T) == typeof(T3) && valueT3 is not null)
        {
            return (T)(object)valueT3;
        }

        throw new InvalidOperationException("Variant value is null.");
    }

    /// <summary>
    /// Try to get the stored value as T. Returns true if successful.
    /// </summary>
    public bool TryGet<T>([NotNullWhen(true)] out T? value)
    {
        if (Is<T>())
        {
            value = Get<T>()!;
            return true;
        }

        value = default;
        return false;
    }
}
