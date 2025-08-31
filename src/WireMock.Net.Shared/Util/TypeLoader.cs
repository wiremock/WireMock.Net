// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Stef.Validation;

namespace WireMock.Util;

internal static class TypeLoader
{
    private static readonly ConcurrentDictionary<string, Type> Assemblies = new();
    private static readonly ConcurrentDictionary<Type, object> Instances = new();
    private static readonly ConcurrentBag<(string FullName, Type Type)> InstancesWhichCannotBeFoundByFullName = [];
    private static readonly ConcurrentBag<(string FullName, Type Type)> StaticInstancesWhichCannotBeFoundByFullName = [];
    private static readonly ConcurrentBag<Type> InstancesWhichCannotBeFound = [];
    private static readonly ConcurrentBag<Type> StaticInstancesWhichCannotBeFound = [];

    public static bool TryLoadNewInstance<TInterface>([NotNullWhen(true)] out TInterface? instance, params object?[] args) where TInterface : class
    {
        var type = typeof(TInterface);
        if (InstancesWhichCannotBeFound.Contains(type))
        {
            instance = null;
            return false;
        }

        if (TryGetPluginType<TInterface>(out var pluginType))
        {
            instance = (TInterface)Activator.CreateInstance(pluginType, args)!;
            return true;
        }

        InstancesWhichCannotBeFound.Add(type);
        instance = null;
        return false;
    }

    public static bool TryLoadStaticInstance<TInterface>([NotNullWhen(true)] out TInterface? staticInstance, params object?[] args) where TInterface : class
    {
        var type = typeof(TInterface);
        if (StaticInstancesWhichCannotBeFound.Contains(type))
        {
            staticInstance = null;
            return false;
        }

        if (TryGetPluginType<TInterface>(out var pluginType))
        {
            staticInstance = (TInterface)Instances.GetOrAdd(pluginType, key => Activator.CreateInstance(key, args)!);
            return true;
        }

        StaticInstancesWhichCannotBeFound.Add(type);
        staticInstance = null;
        return false;
    }

    public static bool TryLoadNewInstanceByFullName<TInterface>([NotNullWhen(true)] out TInterface? instance, string implementationTypeFullName, params object?[] args) where TInterface : class
    {
        Guard.NotNullOrEmpty(implementationTypeFullName);

        var type = typeof(TInterface);
        if (InstancesWhichCannotBeFoundByFullName.Contains((implementationTypeFullName, type)))
        {
            instance = null;
            return false;
        }

        if (TryGetPluginTypeByFullName<TInterface>(implementationTypeFullName, out var pluginType))
        {
            instance = (TInterface)Activator.CreateInstance(pluginType, args)!;
            return true;
        }

        InstancesWhichCannotBeFoundByFullName.Add((implementationTypeFullName, type));
        instance = null;
        return false;
    }

    public static bool TryLoadStaticInstanceByFullName<TInterface>([NotNullWhen(true)] out TInterface? staticInstance, string implementationTypeFullName, params object?[] args) where TInterface : class
    {
        Guard.NotNullOrEmpty(implementationTypeFullName);

        var type = typeof(TInterface);
        if (StaticInstancesWhichCannotBeFoundByFullName.Contains((implementationTypeFullName, type)))
        {
            staticInstance = null;
            return false;
        }

        if (TryGetPluginTypeByFullName<TInterface>(implementationTypeFullName, out var pluginType))
        {
            staticInstance = (TInterface)Instances.GetOrAdd(pluginType, key => Activator.CreateInstance(key, args)!);
            return true;
        }

        StaticInstancesWhichCannotBeFoundByFullName.Add((implementationTypeFullName, type));
        staticInstance = null;
        return false;
    }

    private static bool TryGetPluginType<TInterface>([NotNullWhen(true)] out Type? foundType) where TInterface : class
    {
        var key = typeof(TInterface).FullName!;

        if (Assemblies.TryGetValue(key, out foundType))
        {
            return true;
        }

        if (TryFindTypeInDlls<TInterface>(null, out foundType))
        {
            Assemblies.TryAdd(key, foundType);
            return true;
        }

        return false;
    }

    private static bool TryGetPluginTypeByFullName<TInterface>(string implementationTypeFullName, [NotNullWhen(true)] out Type? foundType) where TInterface : class
    {
        var @interface = typeof(TInterface).FullName;
        var key = $"{@interface}_{implementationTypeFullName}";

        if (Assemblies.TryGetValue(key, out foundType))
        {
            return true;
        }

        if (TryFindTypeInDlls<TInterface>(implementationTypeFullName, out foundType))
        {
            Assemblies.TryAdd(key, foundType);
            return true;
        }

        return false;
    }

    private static bool TryFindTypeInDlls<TInterface>(string? implementationTypeFullName, [NotNullWhen(true)] out Type? pluginType) where TInterface : class
    {
//#if NETSTANDARD1_3
//        var directoriesToSearch = new[] { AppContext.BaseDirectory };
//#else
        var processDirectory = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName);
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var directoriesToSearch = new[] { processDirectory, assemblyDirectory }
            .Where(d => !string.IsNullOrEmpty(d))
            .Distinct()
            .ToArray();
//#endif
        foreach (var directory in directoriesToSearch)
        {
            foreach (var file in Directory.GetFiles(directory!, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName
                    {
                        Name = Path.GetFileNameWithoutExtension(file)
                    });

                    if (TryGetImplementationTypeByInterfaceAndOptionalFullName<TInterface>(assembly, implementationTypeFullName, out pluginType))
                    {
                        return true;
                    }
                }
                catch
                {
                    // no-op: just try next .dll
                }
            }
        }

        pluginType = null;
        return false;
    }

    private static bool TryGetImplementationTypeByInterfaceAndOptionalFullName<T>(Assembly assembly, string? implementationTypeFullName, [NotNullWhen(true)] out Type? type)
    {
        try
        {
            type = assembly
                .GetTypes()
                .FirstOrDefault(t =>
                    typeof(T).IsAssignableFrom(t) && !t.GetTypeInfo().IsInterface &&
                    (implementationTypeFullName == null || string.Equals(t.FullName, implementationTypeFullName, StringComparison.OrdinalIgnoreCase))
                );

            return type != null;
        }
        catch
        {
            type = null;
            return false;
        }
    }
}