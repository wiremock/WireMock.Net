# Copilot Instructions

## Project Guidelines
- All new byte[xx] calls should use using var data = ArrayPool<byte>.Shared.Lease(xx); instead of directly allocating byte arrays