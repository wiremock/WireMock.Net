// Copyright © WireMock.Net

#if NETSTANDARD2_0
namespace WireMock.Compatiblity;

internal static class EnumCompat
{
    public static TEnum Parse<TEnum>(string value) where TEnum : struct, Enum =>  (TEnum)Enum.Parse(typeof(TEnum), value);
}
#endif