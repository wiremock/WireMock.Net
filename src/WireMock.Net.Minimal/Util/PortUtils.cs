// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using WireMock.Constants;

namespace WireMock.Util;

/// <summary>
/// Port Utility class
/// </summary>
internal static class PortUtils
{
    private static readonly Regex UrlDetailsRegex = new(@"^((?<scheme>\w+)://)(?<host>[^/]+?):(?<port>\d+)\/?$", RegexOptions.Compiled, RegexConstants.DefaultTimeout);

    /// <summary>
    /// Finds a random, free port to be listened on.
    /// </summary>
    /// <returns>A random, free port to be listened on.</returns>
    /// <remarks>https://github.com/SeleniumHQ/selenium/blob/trunk/dotnet/src/webdriver/Internal/PortUtilities.cs</remarks>
    public static int FindFreeTcpPort()
    {
        // Locate a free port on the local machine by binding a socket to an IPEndPoint using IPAddress.Any and port 0.
        // The socket will select a free port.
        var portSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            var socketEndPoint = new IPEndPoint(IPAddress.Any, 0);
            portSocket.Bind(socketEndPoint);
            socketEndPoint = (IPEndPoint)portSocket.LocalEndPoint!;
            return socketEndPoint.Port;
        }
        finally
        {
            portSocket.Close();
            portSocket.Dispose();
        }
    }

    /// <summary>
    /// Finds a specified number of random, free ports to be listened on.
    /// </summary>
    /// <param name="count">The number of free ports to find.</param>
    /// <returns>A list of random, free ports to be listened on.</returns>
    public static IReadOnlyList<int> FindFreeTcpPorts(int count)
    {
        var sockets = Enumerable
            .Range(0, count)
            .Select(_ => new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            .ToArray();

        var freePorts = new List<int>();

        try
        {
            foreach (var socket in sockets)
            {
                var socketEndPoint = new IPEndPoint(IPAddress.Any, 0);
                socket.Bind(socketEndPoint);
                socketEndPoint = (IPEndPoint)socket.LocalEndPoint!;

                freePorts.Add(socketEndPoint.Port);
            }

            return freePorts;
        }
        finally
        {
            foreach (var socket in sockets)
            {
                socket.Close();
                socket.Dispose();
            }
        }
    }

    /// <summary>
    /// Extract the isHttps, isHttp2, scheme, host and port from a URL.
    /// </summary>
    public static bool TryExtract(string url, out bool isHttps, out bool isHttp2, [NotNullWhen(true)] out string? scheme, [NotNullWhen(true)] out string? host, out int port)
    {
        isHttps = false;
        isHttp2 = false;
        scheme = null;
        host = null;
        port = 0;

        var match = UrlDetailsRegex.Match(url);
        if (match.Success)
        {
            scheme = match.Groups["scheme"].Value;
            isHttps = scheme.StartsWith("https", StringComparison.OrdinalIgnoreCase) || scheme.StartsWith("grpcs", StringComparison.OrdinalIgnoreCase) || scheme.StartsWith("wss", StringComparison.OrdinalIgnoreCase);
            isHttp2 = scheme.StartsWith("grpc", StringComparison.OrdinalIgnoreCase);
            host = match.Groups["host"].Value;

            return int.TryParse(match.Groups["port"].Value, out port);
        }

        return false;
    }
}