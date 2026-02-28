// Copyright © WireMock.Net

namespace System.Buffers;

internal sealed class Lease<T>(ArrayPool<T> pool, int length) : IDisposable
{
    public T[] Rented { get; } = pool.Rent(length);

    public static implicit operator T[](Lease<T> lease) => lease.Rented;

    public void Dispose()
    {
        pool.Return(Rented, true);
    }
}

internal static class ArrayPoolExtensions
{
    public static Lease<T> Lease<T>(this ArrayPool<T> source, int length) => new(source, length);
}