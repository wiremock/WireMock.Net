## WireMock.Net
Lightweight Http Mocking Server for .NET, inspired by WireMock.org (from the Java landscape).

### :star: Key Features
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

### :star: Stubbing
A core feature of WireMock.Net is the ability to return predefined HTTP responses for requests matching criteria.
See [Stubbing](https://wiremock.org/dotnet/stubbing).

### :star: Request Matching
WireMock.Net support advanced request-matching logic, see [Request Matching](https://wiremock.org/dotnet/request-matching).

### :star: Response Templating
The response which is returned WireMock.Net can be changed using templating. This is described here [Response Templating](https://wiremock.org/dotnet/response-templating).

### :star: Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface see [Admin API Reference](https://wiremock.org/dotnet/admin-api-reference).

### :star: Using
WireMock.Net can be used in several ways:

#### UnitTesting
You can use your favorite test framework and use WireMock within your tests, see
[UnitTesting](https://wiremock.org/dotnet/using-wiremock-in-unittests).

### Unit/Integration Testing using Testcontainers.DotNet
See [WireMock.Net.Testcontainers](https://wiremock.org/dotnet/using-wiremock-net-testcontainers/) on how to build a WireMock.Net Docker container which can be used in Unit/Integration testing.

### Unit/Integration Testing using an an Aspire Distributed Application
See [WireMock.Net.Aspire](https://wiremock.org/dotnet/using-wiremock-net-Aspire) on how to use WireMock.Net as an Aspire Hosted application to do Unit/Integration testing.

#### As a dotnet tool
It's simple to install WireMock.Net as (global) dotnet tool, see [dotnet tool](https://wiremock.org/dotnet/wiremock-as-dotnet-tool).

#### As standalone process / console application
This is quite straight forward to launch a mock server within a console application, see [Standalone Process](https://wiremock.org/dotnet/wiremock-as-a-standalone-process).

#### As a Windows Service
You can also run WireMock.Net as a Windows Service, follow this [WireMock-as-a-Windows-Service](https://wiremock.org/dotnet/wiremock-as-a-Windows-Service).

#### As a Web Job in Azure or application in IIS
See this link [WireMock-as-a-(Azure)-Web-App](https://wiremock.org/dotnet/wiremock-as-a-azure-web-app/)

#### In a docker container
There is also a Linux and Windows-Nano container available at [hub.docker.com](https://hub.docker.com/r/sheyenrath).
For more details see also [Docker](https://github.com/wiremock/WireMock.Net-docker).

#### HTTPS / SSL
More details on using HTTPS (SSL) can be found here [HTTPS](https://wiremock.org/dotnet/using-https-ssl/)

## :books: Documentation
For more info, see also this documentation page: [What is WireMock.Net](https://wiremock.org/dotnet/what-is-wiremock-net/).
