// Copyright © WireMock.Net

namespace WireMock.Compatiblity;

#if NETSTANDARD2_0
internal static class EnumCompat
{
    public static TEnum Parse<TEnum>(string value) where TEnum : struct, Enum =>  (TEnum)Enum.Parse(typeof(TEnum), value);
}
#endif