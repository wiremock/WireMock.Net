# ![Project Icon](./resources/logo_32x32.png) WireMock.Net
A C# .NET version based on [mock4net](https://github.com/alexvictoor/mock4net) which mimics the functionality from the JAVA based [WireMock](http://wiremock.org).

For more info, see also this WIKI page: [What is WireMock.Net](https://github.com/wiremock/WireMock.Net/wiki/What-Is-WireMock.Net).

## :star: Key Features
* HTTP response stubbing, matchable on URL/Path, headers, cookies and body content patterns
* Library can be used in unit tests and integration tests
* Runs as a standalone process, as windows service, as Azure/IIS or as docker
* Configurable via a fluent C# .NET API, JSON files and JSON over HTTP
* Record/playback of stubs (proxying)
* Per-request conditional proxying
* Stateful behaviour simulation
* Response templating / transformation using Handlebars and extensions
* Can be used locally or in CI/CD scenarios
* Can be used for Aspire Distributed Application testing

## :memo: Blogs
- [mstack.nl : Generate C# Code from Mapping(s)](https://mstack.nl/blog/20230201-wiremock.net-tocode)
- [mstack.nl : Chaos Engineering with Fault Injections](https://mstack.nl/blogs/wiremock-net-chaos-engineering-with-fault-injections)
- [mstack.nl : gRPC / ProtoBuf Support](https://mstack.nl/blogs/wiremock-net-grpc)
- [mstack.nl : Build and test your own .NET Aspire component](https://mstack.nl/blogs/wiremock-net-aspire-component/)

## :computer: Project Info
| | |
| --- | --- |
| ***Project*** | &nbsp; |
| &nbsp;&nbsp;**Chat** | [![Slack](https://badgen.net/badge/icon/slack?icon=slack&label)](https://slack.wiremock.org/) [![Gitter](https://img.shields.io/gitter/room/wiremock_dotnet/Lobby.svg)](https://gitter.im/wiremock_dotnet/Lobby) |
| &nbsp;&nbsp;**Issues** | [![GitHub issues](https://img.shields.io/github/issues/WireMock-Net/WireMock.Net.svg)](https://github.com/wiremock/WireMock.Net/issues) |
| | |
| ***Quality*** | &nbsp; |
| &nbsp;&nbsp;**Build Azure** | [![Build Status Azure](https://stef.visualstudio.com/WireMock.Net/_apis/build/status/WireMock.Net)](https://stef.visualstudio.com/WireMock.Net/_build/latest?definitionId=7) |
| &nbsp;&nbsp;**Quality** | [![Sonar Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=alert_status)](https://sonarcloud.io/project/issues?id=WireMock-Net_WireMock.Net) [![CodeFactor](https://www.codefactor.io/repository/github/wiremock/wiremock.net/badge)](https://www.codefactor.io/repository/github/wiremock/wiremock.net) |
| &nbsp;&nbsp;**Sonar Bugs** | [![Sonar Bugs](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=bugs)](https://sonarcloud.io/project/issues?id=WireMock-Net_WireMock.Net&resolved=false&types=BUG) [![Sonar Code Smells](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=code_smells)](https://sonarcloud.io/project/issues?id=WireMock-Net_WireMock.Net&resolved=false&types=CODE_SMELL) |
| &nbsp;&nbsp;**Coverage** | [![Sonar Coverage](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=coverage)](https://sonarcloud.io/component_measures?id=WireMock-Net_WireMock.Net&metric=coverage) [![codecov](https://codecov.io/gh/wiremock/WireMock.Net/branch/master/graph/badge.svg)](https://codecov.io/gh/wiremock/WireMock.Net)|
| &nbsp;&nbsp;**TIOBE** | [TIOBE Quality Indicator](https://ticsdemo.tiobe.com/tiobeweb/DEMO/TqiDashboard.html#axes=Project(WireMock.Net),Sub()&metric=tqi)

### :package: NuGet packages

| | Official | Preview [:information_source:](https://github.com/wiremock/WireMock.Net/wiki/MyGet-preview-versions) |
| - | - | - |
| &nbsp;&nbsp;**WireMock.Net** | [![NuGet Badge WireMock.Net](https://img.shields.io/nuget/v/WireMock.Net)](https://www.nuget.org/packages/WireMock.Net) |  [![MyGet Badge WireMock.Net](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net)
| &nbsp;&nbsp;**WireMock.Net.Minimal** 🔺| [![NuGet Badge WireMock.Net.Minimal](https://img.shields.io/nuget/v/WireMock.Net.Minimal)](https://www.nuget.org/packages/WireMock.Net.Minimal) |  [![MyGet Badge WireMock.Net](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.Minimal?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.Minimal)
| &nbsp;&nbsp;**WireMock.Net.StandAlone** | [![NuGet Badge WireMock.Net](https://img.shields.io/nuget/v/WireMock.Net.StandAlone)](https://www.nuget.org/packages/WireMock.Net.StandAlone) |  [![MyGet Badge WireMock.Net.StandAlone](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.StandAlone?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.StandAlone)
| &nbsp;&nbsp;**WireMock.Net.Testcontainers** | [![NuGet Badge WireMock.Net.Testcontainers](https://img.shields.io/nuget/v/WireMock.Net.Testcontainers)](https://www.nuget.org/packages/WireMock.Net.Testcontainers) | [![MyGet Badge WireMock.Net.Testcontainers](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.Testcontainers?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.Testcontainers)
| &nbsp;&nbsp;**WireMock.Net.Aspire** | [![NuGet Badge WireMock.Net.Aspire](https://img.shields.io/nuget/v/WireMock.Net.Aspire)](https://www.nuget.org/packages/WireMock.Net.Aspire) | [![MyGet Badge WireMock.Net.Aspire](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.Aspire?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.Aspire)
| &nbsp;&nbsp;**WireMock.Net.AspNetCore.Middleware** | [![NuGet Badge WireMock.Net.AspNetCore.Middleware](https://img.shields.io/nuget/v/WireMock.Net.AspNetCore.Middleware)](https://www.nuget.org/packages/WireMock.Net.AspNetCore.Middleware) | [![MyGet Badge WireMock.Net.AspNetCore.Middleware](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.AspNetCore.Middleware?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.AspNetCore.Middleware)
| | | |
| &nbsp;&nbsp;**WireMock.Net.AwesomeAssertions** | [![NuGet Badge WireMock.Net.AwesomeAssertions](https://img.shields.io/nuget/v/WireMock.Net.AwesomeAssertions)](https://www.nuget.org/packages/WireMock.Net.AwesomeAssertions) | [![MyGet Badge WireMock.Net.AwesomeAssertions](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.AwesomeAssertions?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.AwesomeAssertions)
| &nbsp;&nbsp;**WireMock.Net.FluentAssertions** | [![NuGet Badge WireMock.Net.FluentAssertions](https://img.shields.io/nuget/v/WireMock.Net.FluentAssertions)](https://www.nuget.org/packages/WireMock.Net.FluentAssertions) | [![MyGet Badge WireMock.Net.FluentAssertions](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.FluentAssertions?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.FluentAssertions)
| &nbsp;&nbsp;**WireMock.Net.xUnit** | [![NuGet Badge WireMock.Net.xUnit](https://img.shields.io/nuget/v/WireMock.Net.xUnit)](https://www.nuget.org/packages/WireMock.Net.xUnit) | [![MyGet Badge WireMock.Net.xUnit](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.xUnit?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.xUnit)
| &nbsp;&nbsp;**WireMock.Net.TUnit** | [![NuGet Badge WireMock.Net.TUnit](https://img.shields.io/nuget/v/WireMock.Net.TUnit)](https://www.nuget.org/packages/WireMock.Net.TUnit) | [![MyGet Badge WireMock.Net.TUnit](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.TUnit?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.TUnit)
| | | |
| &nbsp;&nbsp;**WireMock.Net.Matchers.CSharpCode** | [![NuGet Badge WireMock.Net.Matchers.CSharpCode](https://img.shields.io/nuget/v/WireMock.Net.Matchers.CSharpCode)](https://www.nuget.org/packages/WireMock.Net.Matchers.CSharpCode) | [![MyGet Badge WireMock.Net.Matchers.CSharpCode](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.Matchers.CSharpCode?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.Matchers.CSharpCode)
| &nbsp;&nbsp;**WireMock.Net.OpenApiParser** | [![NuGet Badge WireMock.Net.OpenApiParser](https://img.shields.io/nuget/v/WireMock.Net.OpenApiParser)](https://www.nuget.org/packages/WireMock.Net.OpenApiParser) | [![MyGet Badge WireMock.Net.OpenApiParser](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.OpenApiParser?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.OpenApiParser)
| &nbsp;&nbsp;**WireMock.Net.MimePart** | [![NuGet Badge WireMock.Net.MimePart](https://img.shields.io/nuget/v/WireMock.Net.MimePart)](https://www.nuget.org/packages/WireMock.Net.MimePart) | [![MyGet Badge WireMock.Net.MimePart](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.MimePart?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.MimePart)
| | | |
| &nbsp;&nbsp;**WireMock.Net.RestClient** | [![NuGet Badge WireMock.Net.RestClient](https://img.shields.io/nuget/v/WireMock.Net.RestClient)](https://www.nuget.org/packages/WireMock.Net.RestClient) | [![MyGet Badge WireMock.Net.RestClient](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.RestClient?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.RestClient)
| &nbsp;&nbsp;**WireMock.Org.RestClient** | [![NuGet Badge WireMock.Org.RestClient](https://img.shields.io/nuget/v/WireMock.Org.RestClient)](https://www.nuget.org/packages/WireMock.Org.RestClient) | [![MyGet Badge WireMock.Org.RestClient](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Org.RestClient?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Org.RestClient)
| &nbsp;&nbsp;**WireMock.Net.Extensions.Routing** | [![NuGet Badge WireMock.Net.Extensions.Routing](https://img.shields.io/nuget/v/WireMock.Net.Extensions.Routing)](https://www.nuget.org/packages/WireMock.Net.Extensions.Routing) | [![MyGet Badge WireMock.Net.Extensions.Routing](https://img.shields.io/myget/wiremock-net/vpre/WireMock.Net.Extensions.Routing?includePreReleases=true&label=MyGet)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.Extensions.Routing)

<br />

🔺 **WireMock.Net.Minimal** does not include: **WireMock.Net.MimePart**

---

## :exclamation: Breaking changes

### 1.7.0
A breaking change is introduced which is related to System.Linq.Dynamic.Core DynamicLinq ([CVE](https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/867)).
- The `LinqMatcher` is not allowed.
- The [Handlebars.Net.Helpers.DynamicLinq](https://www.nuget.org/packages/Handlebars.Net.Helpers.DynamicLinq) package is not included anymore.


### 1.8.0
Some breaking changes are introduced in this version:

#### Handlebars.Net `File`-helper
By default, the internal Handlebars.Net `File`-helper is not allowed anymore because of potential security issues.
To still enable this feature, you need to set the `AllowedCustomHandlebarHelpers` property to `File` in the `HandlebarsSettings` property in `WireMockServerSettings`.

#### Handlebars.Net `Environment`-helper
By default, the Handlebars.Net `Environment`-helper is not automatically allowed anymore because of potential security issues.
To still enable this feature, you need to add the `Environment` category to the `AllowedHandlebarsHelpers` list-property in the `HandlebarsSettings` property in `WireMockServerSettings`.

---

## :memo: Development
For the supported frameworks and build information, see [this](https://github.com/wiremock/WireMock.Net/wiki/Development-Information) page.

## :star: Stubbing
A core feature of WireMock.Net is the ability to return predefined HTTP responses for requests matching criteria.
See [Wiki : Stubbing](https://github.com/wiremock/WireMock.Net/wiki/Stubbing).

## :star: Request Matching
WireMock.Net support advanced request-matching logic, see [Wiki : Request Matching](https://github.com/wiremock/WireMock.Net/wiki/Request-Matching).

## :star: Response Templating
The response which is returned WireMock.Net can be changed using templating. This is described here [Wiki : Response Templating](https://github.com/wiremock/WireMock.Net/wiki/Response-Templating).

## :star: Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface see [Wiki : Admin API Reference](https://github.com/StefH/WireMock.Net/wiki/Admin-API-Reference).

## :star: Using
WireMock.Net can be used in several ways:

### UnitTesting
You can use your favorite test framework and use WireMock within your tests, see
[Wiki : UnitTesting](https://github.com/StefH/WireMock.Net/wiki/Using-WireMock-in-UnitTests).

### Unit/Integration Testing using Testcontainers.DotNet
See [Wiki : WireMock.Net.Testcontainers](https://github.com/wiremock/WireMock.Net/wiki/Using-WireMock.Net.Testcontainers) on how to build a WireMock.Net Docker container which can be used in Unit/Integration testing.

### Unit/Integration Testing using an an Aspire Distributed Application
See [Wiki : WireMock.Net.Aspire](https://github.com/wiremock/WireMock.Net/wiki/Using-WireMock.Net.Aspire) on how to use WireMock.Net as an Aspire Hosted application to do Unit/Integration testing.

### As a dotnet tool
It's simple to install WireMock.Net as (global) dotnet tool, see [Wiki : dotnet tool](https://github.com/StefH/WireMock.Net/wiki/WireMock-as-dotnet-tool).

### As standalone process / console application
This is quite straight forward to launch a mock server within a console application, see [Wiki : Standalone Process](https://github.com/StefH/WireMock.Net/wiki/WireMock-as-a-standalone-process).

### As a Windows Service
You can also run WireMock.Net as a Windows Service, follow this [WireMock-as-a-Windows-Service](https://github.com/wiremock/WireMock.Net/wiki/WireMock-as-a-Windows-Service).

### As a Web Job in Azure or application in IIS
See this link [WireMock-as-a-(Azure)-Web-App](https://github.com/wiremock/WireMock.Net/wiki/WireMock-as-a-(Azure)-Web-App)

### In a docker container
There is also a Linux and Windows-Nano container available at [hub.docker.com](https://hub.docker.com/r/sheyenrath).
For more details see also [Docker](https://github.com/wiremock/WireMock.Net-docker).

#### HTTPS / SSL
More details on using HTTPS (SSL) can be found here [Wiki : HTTPS](https://github.com/wiremock/WireMock.Net/wiki/Using-HTTPS-(SSL))

---

## Powered by
[![JetBrains logo.](https://resources.jetbrains.com/storage/products/company/brand/logos/jetbrains.svg)](https://jb.gg/OpenSource)
