// Copyright © WireMock.Net

namespace WireMock.Types;

[Flags]
public enum QueryParameterMultipleValueSupport
{
    // Support none
    None = 0x0,

    // Support "&" as multi-value-separator --> "key=value&key=anotherValue"
    Ampersand = 0x1,

    // Support ";" as multi-value-separator --> "key=value;key=anotherValue"
    SemiColon = 0x2,

    // Support "," as multi-value-separator -->  "key=1,2"
    Comma = 0x4,

    // Support "&" and ";" as multi-value-separator --> "key=value&key=anotherValue" and also "key=value;key=anotherValue"
    AmpersandAndSemiColon = Ampersand | SemiColon,

    // Support "&" and ";" as multi-value-separator
    NoComma = AmpersandAndSemiColon,

    // Support all multi-value-separators ("&" and ";" and ",")
    All = Ampersand | SemiColon | Comma
}