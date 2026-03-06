// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;

namespace WireMock.Models;

/// <inheritdoc />
internal class BlockingQueue<T>(TimeSpan? readTimeout = null) : IBlockingQueue<T>
{
    private readonly TimeSpan _readTimeout = readTimeout ?? TimeSpan.FromHours(1);
    private readonly Queue<T?> _queue = new();
    private readonly object _lockObject = new();

    private bool _isClosed;

    /// <summary>
    /// Writes an item to the queue and signals that an item is available.
    /// </summary>
    /// <param name="item">The item to be added to the queue.</param>
    public void Write(T item)
    {
        lock (_lockObject)
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("Cannot write to a closed queue.");
            }

            _queue.Enqueue(item);

            // Signal that an item is available
            Monitor.Pulse(_lockObject);
        }
    }

    /// <summary>
    /// Tries to read an item from the queue.
    /// - waits until an item is available
    /// - or the timeout occurs
    /// - or queue is closed
    /// </summary>
    /// <param name="item">The item read from the queue, or default if the timeout occurs.</param>
    /// <returns>True if an item was successfully read; otherwise, false.</returns>
    public bool TryRead([NotNullWhen(true)] out T? item)
    {
        lock (_lockObject)
        {
            // Wait until an item is available or timeout occurs
            while (_queue.Count == 0 && !_isClosed)
            {
                // Wait with timeout
                if (!Monitor.Wait(_lockObject, _readTimeout))
                {
                    item = default;
                    return false;
                }
            }

            // After waiting, check if we have items
            if (_queue.Count == 0)
            {
                item = default;
                return false;
            }

            item = _queue.Dequeue();
            return item != null;
        }
    }

    /// <summary>
    /// Closes the queue and signals all waiting threads.
    /// </summary>
    public void Close()
    {
        lock (_lockObject)
        {
            _isClosed = true;
            Monitor.PulseAll(_lockObject);
        }
    }
}