# LINQPadHelpers

A NuGet package providing common utility functionality for use in [LINQPad](https://www.linqpad.net/) scripts and queries. Consolidates reusable helpers — extensions, controls, logging, Azure utilities, and more — so they can be easily referenced across multiple queries.

## Getting Started

1. **Clone the repository** to your local machine.
2. **Build the solution** in Visual Studio or via `dotnet build`.
3. **Reference in LINQPad:** Press `F4` in a query to open query properties, click **Add NuGet...**, search for `LINQPadHelpers`, and add it.
4. Click **Add namespaces** on the green NuGet entry and select the namespaces you want.

---

## Contents

- **Azure**
  - `KeyVaultSecrets` — wrapper for Azure Key Vault access.
  - `LINQPadTokenCredential` — Azure credential that uses your LINQPad-authenticated identity.
- **Controls** — UI controls and related extensions for visual LINQPad scripts:
  - `CheckboxList`, `CheckboxSelector`, `RadioSelector`
  - `DirectoryFilePicker`
  - `EvalList`, `LatestDumpContainer`
  - `ITableDisplay`
  - Extension methods on controls and objects
- **Extensions** — A broad set of extension methods covering strings, collections, files, XML, JSON, tasks, enums, and more.
- **Logging** — An `ILogger` / `ILoggerProvider` implementation that renders log output nicely in the LINQPad results panel.
- **Misc**
  - `TraceLogHandler` — `HttpMessageHandler` that logs HTTP request/response to an `ILogger`.
  - `HttpClientBuilderExtensions` — extension to wire up the trace handler.
- **PInvoke** — Win32 utilities: current user UPN lookup, shell-association icon display.
