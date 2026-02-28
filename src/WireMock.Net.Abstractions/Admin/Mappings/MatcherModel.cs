// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// MatcherModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class MatcherModel
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the pattern. Can be a string (default) or an object.
    /// </summary>
    public object? Pattern { get; set; }

    /// <summary>
    /// Gets or sets the patterns. Can be an array of strings (default) or an array of objects.
    /// </summary>
    public object[]? Patterns { get; set; }

    /// <summary>
    /// Gets or sets the pattern as a file.
    /// </summary>
    public string? PatternAsFile { get; set; }

    /// <summary>
    /// Gets or sets the ignore case.
    /// </summary>
    public bool? IgnoreCase { get; set; }

    /// <summary>
    /// Reject on match.
    /// </summary>
    public bool? RejectOnMatch { get; set; }

    /// <summary>
    /// The Operator to use when multiple patterns are defined. Optional.
    /// - null      = Same as "or".
    /// - "or"      = Only one pattern is required to match.
    /// - "and"     = All patterns should match.
    /// - "average" = The average value from all patterns.
    /// </summary>
    public string? MatchOperator { get; set; }

    #region JsonMatcher, JsonPartialMatcher and JsonPartialWildcardMatcher
    /// <summary>
    /// Support Regex.
    /// </summary>
    public bool? Regex { get; set; }
    #endregion

    #region MimePartMatcher
    /// <summary>
    /// ContentType Matcher (image/png; name=image.png)
    /// </summary>
    public MatcherModel? ContentTypeMatcher { get; set; }

    /// <summary>
    /// ContentDisposition Matcher (attachment; filename=image.png)
    /// </summary>
    public MatcherModel? ContentDispositionMatcher { get; set; }

    /// <summary>
    /// ContentTransferEncoding Matcher (base64)
    /// </summary>
    public MatcherModel? ContentTransferEncodingMatcher { get; set; }
    #endregion

    #region MimePartMatcher + ProtoBufMatcher
    /// <summary>
    /// Content Matcher
    /// </summary>
    public MatcherModel? ContentMatcher { get; set; }
    #endregion

    #region ProtoBufMatcher
    /// <summary>
    /// The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".
    /// </summary>
    public string? ProtoBufMessageType { get; set; }
    #endregion

    #region XPathMatcher
    /// <summary>
    /// Array of namespace prefix and uri. (optional)
    /// </summary>
    public XmlNamespace[]? XmlNamespaceMap { get; set; }
    #endregion

    #region GraphQLMatcher
    /// <summary>
    /// Mapping of custom GraphQL Scalar name to ClrType. (optional)
    /// </summary>
    public IDictionary<string, Type>? CustomScalars { get; set; }
    #endregion
}