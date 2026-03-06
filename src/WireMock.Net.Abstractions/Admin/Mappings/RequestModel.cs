// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// RequestModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class RequestModel
{
    /// <summary>
    /// Gets or sets the ClientIP. (Can be a string or a ClientIPModel)
    /// </summary>
    public object? ClientIP { get; set; }

    /// <summary>
    /// Gets or sets the Path. (Can be a string or a PathModel)
    /// </summary>
    public object? Path { get; set; }

    /// <summary>
    /// Gets or sets the Url. (Can be a string or a UrlModel)
    /// </summary>
    public object? Url { get; set; }

    /// <summary>
    /// The methods
    /// </summary>
    public string[]? Methods { get; set; }

    /// <summary>
    /// The HTTP Version
    /// </summary>
    public string? HttpVersion { get; set; }

    /// <summary>
    /// Reject on match for Methods.
    /// </summary>
    public bool? MethodsRejectOnMatch { get; set; }

    /// <summary>
    /// The Operator to use when Methods are defined. [Optional]
    /// - null      = Same as "or".
    /// - "or"      = Only one method should match.
    /// - "and"     = All methods should match.
    /// - "average" = The average value from all methods.
    /// </summary>
    public string? MethodsMatchOperator { get; set; }

    /// <summary>
    /// Gets or sets the Headers.
    /// </summary>
    public IList<HeaderModel>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the Cookies.
    /// </summary>
    public IList<CookieModel>? Cookies { get; set; }

    /// <summary>
    /// Gets or sets the Params.
    /// </summary>
    public IList<ParamModel>? Params { get; set; }
        
    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    public BodyModel? Body { get; set; }
}