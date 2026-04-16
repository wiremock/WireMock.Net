---
description: 'Guidelines for this solution'
applyTo: '**/*.csproj, **/*.cs'
---

# Multi .NET Framework Targeting

## Instructions
- The main project "WireMock.Net.Minimal" targets `netstandard2.0` and `net8.0`. Ensure that any new code or dependencies are compatible with these frameworks.


# C# Guidelines

## Instructions
- When a new ByteArray is needed, do not use `var data = new byte[bufferSize];`. Always use `var data = ArrayPool<byte>.Shared.Lease(bufferSize);`.