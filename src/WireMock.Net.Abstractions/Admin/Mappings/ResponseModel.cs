// Copyright © WireMock.Net

using WireMock.Admin.Settings;

namespace WireMock.Admin.Mappings;

/// <summary>
/// ResponseModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class ResponseModel
{
    /// <summary>
    /// Gets or sets the HTTP status.
    /// </summary>
    public object? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the body destination (SameAsSource, String or Bytes).
    /// </summary>
    public string? BodyDestination { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Gets or sets the body (as JSON object).
    /// </summary>
    public object? BodyAsJson { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Json Body String needs to be indented.
    /// </summary>
    public bool? BodyAsJsonIndented { get; set; }

    /// <summary>
    /// Gets or sets the body (as byte array).
    /// </summary>
    public byte[]? BodyAsBytes { get; set; }

    /// <summary>
    /// Gets or sets the body as a file.
    /// </summary>
    public string? BodyAsFile { get; set; }

    /// <summary>
    /// Is the body as file cached?
    /// </summary>
    public bool? BodyAsFileIsCached { get; set; }

    /// <summary>
    /// Gets or sets the body encoding.
    /// </summary>
    public EncodingModel? BodyEncoding { get; set; }

    /// <summary>
    /// Use ResponseMessage Transformer.
    /// </summary>
    public bool? UseTransformer { get; set; }

    /// <summary>
    /// Gets the type of the transformer.
    /// </summary>
    public string? TransformerType { get; set; }

    /// <summary>
    /// Use the Handlebars transformer for the content from the referenced BodyAsFile.
    /// </summary>
    public bool? UseTransformerForBodyAsFile { get; set; }

    /// <summary>
    /// The ReplaceNodeOptions to use when transforming a JSON node.
    /// </summary>
    public string? TransformerReplaceNodeOptions { get; set; }

    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public IDictionary<string, object>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the Headers (Raw).
    /// </summary>
    public string? HeadersRaw { get; set; }

    /// <summary>
    /// Gets or sets the Trailing Headers.
    /// </summary>
    public IDictionary<string, object>? TrailingHeaders { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds.
    /// </summary>
    public int? Delay { get; set; }

    /// <summary>
    /// Gets or sets the minimum random delay in milliseconds.
    /// </summary>
    public int? MinimumRandomDelay { get; set; }

    /// <summary>
    /// Gets or sets the maximum random delay in milliseconds.
    /// </summary>
    public int? MaximumRandomDelay { get; set; }

    /// <summary>
    /// Gets or sets the Proxy URL.
    /// </summary>
    public string? ProxyUrl { get; set; }

    /// <summary>
    /// Defines the Proxy Url Replace Settings.
    /// </summary>
    public ProxyUrlReplaceSettingsModel? ProxyUrlReplaceSettings { get; set; }

    /// <summary>
    /// The client X509Certificate2 Thumbprint or SubjectName to use.
    /// </summary>
    public string? X509Certificate2ThumbprintOrSubjectName { get; set; }

    /// <summary>
    /// Gets or sets the fault.
    /// </summary>
    public FaultModel? Fault { get; set; }

    /// <summary>
    /// Gets or sets the WebProxy settings.
    /// </summary>
    public WebProxyModel? WebProxy { get; set; }

    #region ProtoBuf
    /// <summary>
    /// Gets or sets the proto definition.
    /// </summary>
    public string? ProtoDefinition { get; set; }

    /// <summary>
    /// Gets or sets the proto definitions.
    /// </summary>
    public string[]? ProtoDefinitions { get; set; }

    /// <summary>
    /// Gets or sets the full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".
    /// </summary>
    public string? ProtoBufMessageType { get; set; }
    #endregion
}