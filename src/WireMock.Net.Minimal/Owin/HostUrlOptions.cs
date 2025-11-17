// Copyright © WireMock.Net

using System.Collections.Generic;
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
            if (HostingScheme is HostingScheme.Http or HostingScheme.Https)
            {
                var port = Port > 0 ? Port.Value : FindFreeTcpPort();
                var scheme = HostingScheme == HostingScheme.Https ? "https" : "http";
                list.Add(new HostUrlDetails { IsHttps = HostingScheme == HostingScheme.Https, IsHttp2 = UseHttp2 == true, Url = $"{scheme}://{Star}:{port}", Scheme = scheme, Host = Star, Port = port });
            }

            if (HostingScheme == HostingScheme.HttpAndHttps)
            {
                var httpPort = Port > 0 ? Port.Value : FindFreeTcpPort();
                list.Add(new HostUrlDetails { IsHttps = false, IsHttp2 = UseHttp2 == true, Url = $"http://{Star}:{httpPort}", Scheme = "http", Host = Star, Port = httpPort });

                var httpsPort = FindFreeTcpPort(); // In this scenario, always get a free port for https.
                list.Add(new HostUrlDetails { IsHttps = true, IsHttp2 = UseHttp2 == true, Url = $"https://{Star}:{httpsPort}", Scheme = "https", Host = Star, Port = httpsPort });
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

    private static int FindFreeTcpPort()
    {
//#if USE_ASPNETCORE || NETSTANDARD2_0 || NETSTANDARD2_1
        return 0;
//#else
        //return PortUtils.FindFreeTcpPort();
//#endif
    }
}