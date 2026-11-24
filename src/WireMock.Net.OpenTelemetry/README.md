# WireMock.Net.OpenTelemetry

OpenTelemetry tracing support for WireMock.Net. This package provides instrumentation and OTLP (OpenTelemetry Protocol) exporting capabilities.

## Overview

WireMock.Net automatically creates `System.Diagnostics.Activity` objects for request tracing when `ActivityTracingOptions` is configured (not null). These activities use the built-in .NET distributed tracing infrastructure and are available without any additional dependencies.

This package provides:
- **WireMock.Net instrumentation** - Adds the WireMock.Net ActivitySource to the tracing pipeline
- **ASP.NET Core instrumentation** - Standard HTTP server tracing with request filtering
- **OTLP exporter** - Sends traces to an OpenTelemetry collector

## Installation

```bash
dotnet add package WireMock.Net.OpenTelemetry
```

## Usage

### Option 1: Using AdditionalServiceRegistration (Recommended)

```csharp
using WireMock.OpenTelemetry;
using WireMock.Server;
using WireMock.Settings;

var openTelemetryOptions = new OpenTelemetryOptions
{
    ActivitySourceName = "MyService",
    ExcludeAdminRequests = true,
    OtlpExporterEndpoint = "http://localhost:4317" // Your OTEL collector
};

var settings = new WireMockServerSettings
{
    // Setting ActivityTracingOptions (not null) enables activity creation in middleware
    ActivityTracingOptions = new ActivityTracingOptions
    {
        ExcludeAdminRequests = true,
        RecordRequestBody = false, // PII concern
        RecordResponseBody = false, // PII concern
        RecordMatchDetails = true
    },
    AdditionalServiceRegistration = services =>
    {
        services.AddWireMockOpenTelemetry(openTelemetryOptions);
    }
};

var server = WireMockServer.Start(settings);
```

### Option 2: Custom TracerProvider Configuration

For more control over the tracing configuration:

```csharp
using OpenTelemetry;
using OpenTelemetry.Trace;
using WireMock.OpenTelemetry;

var openTelemetryOptions = new OpenTelemetryOptions();

// Configure your own TracerProvider
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddWireMockInstrumentation(openTelemetryOptions) // Adds WireMock.Net and the configured source
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
    })
    .Build();
```

## Extension Methods

### `AddWireMockOpenTelemetry`

Adds full OpenTelemetry tracing to the service collection with instrumentation and OTLP exporter:

```csharp
services.AddWireMockOpenTelemetry(openTelemetryOptions);
```

This configures:
- The WireMock.Net ActivitySource and the configured ActivitySource
- A resource with the configured service name
- ASP.NET Core instrumentation
- OTLP exporter (using the endpoint from `OpenTelemetryOptions.OtlpExporterEndpoint` or the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable)

### `AddWireMockInstrumentation`

Adds WireMock instrumentation to an existing TracerProviderBuilder:

```csharp
tracerProvider.AddWireMockInstrumentation(openTelemetryOptions);
```

## Configuration

### OpenTelemetryOptions (Exporter configuration)

| Property | Description | Default |
|----------|-------------|---------|
| `ActivitySourceName` | Additional ActivitySource to subscribe to and service name for `AddWireMockOpenTelemetry` | `WireMock.Net` |
| `ExcludeAdminRequests` | Exclude `/__admin/*` from ASP.NET Core instrumentation | `true` |
| `OtlpExporterEndpoint` | OTLP collector endpoint URL | Uses `OTEL_EXPORTER_OTLP_ENDPOINT` env var |

WireMock's own activities always use the `WireMock.Net` source, which remains subscribed even when a different source is configured. `AddWireMockInstrumentation` only configures instrumentation; configure its resource separately if you need a custom service name.

### ActivityTracingOptions (Trace content configuration)

Configured in `WireMockServerSettings.ActivityTracingOptions`:

| Property | Description | Default |
|----------|-------------|---------|
| `ExcludeAdminRequests` | Exclude `/__admin/*` from WireMock activity creation | `true` |
| `RecordRequestBody` | Include request body in trace attributes | `false` |
| `RecordResponseBody` | Include response body in trace attributes | `false` |
| `RecordMatchDetails` | Include mapping match details in trace attributes | `true` |

## Trace Attributes

WireMock.Net traces include these semantic conventions:

**Standard HTTP attributes:**
- `http.request.method`
- `url.full`
- `url.path`
- `server.address`
- `http.response.status_code`
- `client.address`

**WireMock-specific attributes:**
- `wiremock.mapping.matched` - Whether a mapping was found
- `wiremock.mapping.guid` - GUID of the matched mapping
- `wiremock.mapping.title` - Title of the matched mapping
- `wiremock.match.score` - Match score
- `wiremock.request.guid` - GUID of the request

## CLI Arguments

When using WireMock.Net.StandAlone or Docker images, activity tracing and OpenTelemetry can be configured via command-line arguments:

**Activity Tracing (what gets recorded):**
```bash
--ActivityTracingEnabled true
--ActivityTracingExcludeAdminRequests true
--ActivityTracingRecordRequestBody false
--ActivityTracingRecordResponseBody false
--ActivityTracingRecordMatchDetails true
```

**OpenTelemetry Export (where traces are sent):**
```bash
--OpenTelemetryEnabled true
--OpenTelemetryActivitySourceName MyService
--OpenTelemetryOtlpExporterEndpoint http://localhost:4317
--OpenTelemetryExcludeAdminRequests true
```

For Docker or standalone environment configuration, set `WireMockServerSettings__OpenTelemetryEnabled=true` and `WireMockServerSettings__OpenTelemetryActivitySourceName=MyService`. The activity source name defaults to `WireMock.Net` when omitted or empty.

## Requirements

- .NET 8.0 or later
- WireMock.Net 1.6.0 or later
