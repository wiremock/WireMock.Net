// Copyright © WireMock.Net

namespace WireMock.Types;

/// <summary>
/// A special List which overrides the ToString() to return first value in case of a single element.
/// Else it will return a comma separated list of all values.
/// If null or empty, it will return an empty string.
/// </summary>
/// <typeparam name="T">The generic type</typeparam>
/// <seealso cref="List{T}" />
public class WireMockList<T> : List<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockList{T}"/> class.
    /// </summary>
    public WireMockList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockList{T}"/> class.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    public WireMockList(params T[] collection) : base(collection)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockList{T}"/> class.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    public WireMockList(IEnumerable<T> collection) : base(collection)
    {
    }

    /// <summary>
    /// Operator for setting T
    /// </summary>
    /// <param name="value">The value to set.</param>
    public static implicit operator WireMockList<T>(T value) => new(value);

    /// <summary>
    /// Operator for setting T[]
    /// </summary>
    /// <param name="values">The values to set.</param>
    public static implicit operator WireMockList<T>(T[] values) => new(values);

    /// <summary>
    /// Operator for equality comparison from WireMockList to T
    /// </summary>
    public static bool operator ==(WireMockList<T>? left, T? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left?.Count == 1 && Equals(left[0], right))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Operator for equality comparison from T to WireMockList
    /// </summary>
    public static bool operator ==(T? left, WireMockList<T>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (right?.Count == 1 && Equals(left, right[0]))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Operator for inequality comparison from WireMockList to T
    /// </summary>
    public static bool operator !=(WireMockList<T>? left, T? right) => !(left == right);

    /// <summary>
    /// Operator for inequality comparison from T to WireMockList
    /// </summary>
    public static bool operator !=(T? left, WireMockList<T>? right) => !(left == right);

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// Two <see cref="WireMockList{T}"/> instances are equal if they contain the same elements in the same order.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is WireMockList<T> other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                if (!Equals(this[i], other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns a hash code for this instance based on its elements.
    /// </summary>
    public override int GetHashCode()
    {
        var hashCode = 17;
        foreach (var item in this)
        {
            hashCode = hashCode * 31 + (item?.GetHashCode() ?? 0);
        }

        return hashCode;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    public override string ToString()
    {
        switch (Count)
        {
            case 0:
                return string.Empty;

            case 1:
                if (this[0] is string strValue)
                {
                    return strValue;
                }

                return this[0]?.ToString() ?? string.Empty;

            default:
                var strings = this.Select(x => x as string ?? x?.ToString() ?? string.Empty);
                return string.Join(", ", strings);
        }
    }
}