// Copyright © WireMock.Net

namespace WireMock.Util;

internal static class SingletonLock
{
    public static readonly object Lock = new();
}

internal static class SingletonFactory<T> where T : class, new()
{
    private static T? _instance;

    public static T GetInstance()
    {
        if (_instance == null)
        {
            lock (SingletonLock.Lock)
            {
                _instance ??= new T();
            }
        }

        return _instance;
    }
}