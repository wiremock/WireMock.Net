# WireMock.Net.Extensions.Routing

**WireMock.Net.Extensions.Routing** extends [WireMock.Net](https://github.com/wiremock/wiremock) with modern, minimal-API-style routing for .NET. It provides extension methods for expressive, maintainable, and testable HTTP routing, inspired by [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-9.0).

---

## Motivation

While [WireMock.Net](https://github.com/WireMock-Net/WireMock.Net) is a powerful tool for HTTP mocking in .NET, its native API for defining routes and request handlers can be verbose and require significant boilerplate. Setting up even simple endpoints often involves multiple chained method calls, manual parsing of request data, and repetitive configuration, which can make tests harder to read and maintain.

**WireMock.Net.Extensions.Routing** addresses these pain points by introducing a concise, fluent, and minimal-API-inspired approach to routing. This makes your test code:

- **More readable:** Route definitions are clear and expressive, closely resembling production minimal APIs.
- **Easier to maintain:** Less boilerplate means fewer places for errors and easier refactoring.
- **Faster to write:** Define routes and handlers in a single line, with strong typing and async support.

### Example: Native WireMock.Net vs. WireMock.Net.Extensions.Routing

#### Native WireMock.Net

```csharp
server.Given(
    Request.Create().WithPath("/hello").UsingGet()
)
.RespondWith(
    Response.Create().WithBody("Hello, world!")
);

server.Given(
    Request.Create().WithPath("/user/*").UsingGet()
)
.RespondWith(
    Response.Create().WithCallback(request =>
    {
        var id = request.PathSegments[1];
        // ...fetch user by id...
        return new ResponseMessage { Body = $"User: {id}" };
    })
);
```

#### With WireMock.Net.Extensions.Routing

```csharp
router.MapGet("/hello", _ => "Hello, world!");

router.MapGet("/user/{id:int}", requestInfo =>
{
    var id = requestInfo.RouteArgs["id"];
    // ...fetch user by id...
    return $"User: {id}";
});
```

With **WireMock.Net.Extensions.Routing**, you get:

- Minimal, one-line route definitions
- Typed route parameters (e.g., `{id:int}`)
- Direct access to parsed route arguments and request bodies
- Async handler support

This leads to more maintainable, scalable, and production-like test code.

---

## Features

- Minimal API-style route definitions for WireMock.Net
- Strongly-typed request handling
- Routing parameters with constraints (`int` and `string` are currently supported)
- Asynchronous handlers
- Fluent, composable routing extensions
- Easy integration with existing WireMock.Net servers
- .NET 8+ support

---

## Installation

Install from NuGet:

```shell
dotnet add package WireMock.Net.Extensions.Routing
```

---

## Quick Start

```csharp
using System.Net.Http.Json;
using WireMock.Net.Extensions.Routing;
using WireMock.Net.Extensions.Routing.Extensions;
using WireMock.Server;

var server = WireMockServer.Start();
var router = new WireMockRouter(server);

router.MapGet("/hello", _ => "Hello, world!");

using var client = server.CreateClient();
var result = await client.GetFromJsonAsync<string>("/hello");
// Hello, world!
```

---

## Usage

### Routing with route parameters

```csharp
router.MapGet("/user/{id:int}", async requestInfo => {
    var userId = requestInfo.RouteArgs["id"];
    // var user = await ...
    return user;
});
```

### Strongly-Typed Request Info

```csharp
router.MapPost<Item>("/api/items", requestInfo => {
    var item = requestInfo.Body!;
    // process item
    return Results.Json(new { success = true });
});
```

### Supported Methods

- `MapGet`, `MapPost`, `MapPut`, `MapDelete`
---

## Documentation

- [API Reference](./src/WireMock.Net.Extensions.Routing/)
- [WireMock.Net Documentation](https://github.com/WireMock-Net/WireMock.Net)
