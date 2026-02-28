// Copyright © WireMock.Net

using WireMock.Types;
using WireMock.Util;

namespace WireMock.Owin;

internal class HostUrlOptions
{
    private const string Star = "*";

    public ICollection<string>? Urls { get; set; }

    public int? Port { get; set; }

    public HostingScheme HostingScheme { get; set; }

    public bool? UseHttp2 { get; set; }

    public IReadOnlyList<HostUrlDetails> GetDetails()
    {
        var list = new List<HostUrlDetails>();
        if (Urls == null)
        {
            if (HostingScheme is not HostingScheme.None)
            {
                var scheme = GetSchemeAsString(HostingScheme);
                var port = Port > 0 ? Port.Value : 0;
                var isHttps = HostingScheme == HostingScheme.Https || HostingScheme == HostingScheme.Wss;
                list.Add(new HostUrlDetails { IsHttps = isHttps, IsHttp2 = UseHttp2 == true, Url = $"{scheme}://{Star}:{port}", Scheme = scheme, Host = Star, Port = port });
            }

            if (HostingScheme == HostingScheme.HttpAndHttps)
            {
                var port = Port > 0 ? Port.Value : 0;
                var scheme = GetSchemeAsString(HostingScheme.Http);
                list.Add(new HostUrlDetails { IsHttps = false, IsHttp2 = UseHttp2 == true, Url = $"{scheme}://{Star}:{port}", Scheme = scheme, Host = Star, Port = port });

                var securePort = 0; // In this scenario, always get a free port for https.
                var secureScheme = GetSchemeAsString(HostingScheme.Https);
                list.Add(new HostUrlDetails { IsHttps = true, IsHttp2 = UseHttp2 == true, Url = $"{secureScheme}://{Star}:{securePort}", Scheme = secureScheme, Host = Star, Port = securePort });
            }

            if (HostingScheme == HostingScheme.WsAndWss)
            {
                var port = Port > 0 ? Port.Value : 0;
                var scheme = GetSchemeAsString(HostingScheme.Ws);
                list.Add(new HostUrlDetails { IsHttps = false, IsHttp2 = UseHttp2 == true, Url = $"{scheme}://{Star}:{port}", Scheme = scheme, Host = Star, Port = port });

                var securePort = 0; // In this scenario, always get a free port for https.
                var secureScheme = GetSchemeAsString(HostingScheme.Wss);
                list.Add(new HostUrlDetails { IsHttps = true, IsHttp2 = UseHttp2 == true, Url = $"{secureScheme}://{Star}:{securePort}", Scheme = secureScheme, Host = Star, Port = securePort });
            }
        }
        else
        {
            foreach (var url in Urls)
            {
                if (PortUtils.TryExtract(url, out var isHttps, out var isGrpc, out var protocol, out var host, out var port))
                {
                    list.Add(new HostUrlDetails { IsHttps = isHttps, IsHttp2 = isGrpc, Url = url, Scheme = protocol, Host = host, Port = port });
                }
            }
        }

        return list;
    }

    private string GetSchemeAsString(HostingScheme scheme)
    {
        return scheme switch
        {
            HostingScheme.Http => "http",
            HostingScheme.Https => "https",
            HostingScheme.HttpAndHttps => "http", // Default to http when both are specified, since the https URL will be added separately with a free port.

            HostingScheme.Ws => "ws",
            HostingScheme.Wss => "wss",
            HostingScheme.WsAndWss => "ws", // Default to ws when both are specified, since the wss URL will be added separately with a free port.

            _ => throw new NotSupportedException($"Unsupported hosting scheme: {HostingScheme}")
        };
    }
}