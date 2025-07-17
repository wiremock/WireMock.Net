// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace WireMock.Util;

internal static class ReflectionUtils
{
    private const string DynamicModuleName = "WireMockDynamicModule";
    private static readonly AssemblyName AssemblyName = new("WireMockDynamicAssembly");
    private const TypeAttributes ClassAttributes =
        TypeAttributes.Public |
        TypeAttributes.Class |
        TypeAttributes.AutoClass |
        TypeAttributes.AnsiClass |
        TypeAttributes.BeforeFieldInit |
        TypeAttributes.AutoLayout;
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

    public static Type CreateType(string typeName, Type? parentType = null)
    {
        return TypeCache.GetOrAdd(typeName, key =>
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);

            var typeBuilder = moduleBuilder.DefineType(key, ClassAttributes, parentType);

            // Create the type and cache it
            return typeBuilder.CreateTypeInfo()!.AsType();
        });
    }

    public static Type CreateGenericType(string typeName, Type genericTypeDefinition, params Type[] typeArguments)
    {
        var genericKey = $"{typeName}_{genericTypeDefinition.Name}_{string.Join(", ", typeArguments.Select(t => t.Name))}";

        return TypeCache.GetOrAdd(genericKey, _ =>
        {
            var genericType = genericTypeDefinition.MakeGenericType(typeArguments);

            // Create the type based on the genericType and cache it
            return CreateType(typeName, genericType);
        });
    }
}