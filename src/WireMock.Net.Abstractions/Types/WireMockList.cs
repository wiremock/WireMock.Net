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