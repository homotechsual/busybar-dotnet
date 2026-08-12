# BusyBar

[![CI](https://img.shields.io/github/actions/workflow/status/homotechsual/busybar-dotnet/ci.yml?branch=main&style=for-the-badge&label=CI)](https://github.com/homotechsual/busybar-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/BusyBar?style=for-the-badge&label=NuGet)](https://www.nuget.org/packages/BusyBar)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BusyBar?style=for-the-badge&label=Downloads)](https://www.nuget.org/packages/BusyBar)
[![Codecov](https://img.shields.io/codecov/c/github/homotechsual/busybar-dotnet?style=for-the-badge&label=Coverage)](https://codecov.io/gh/homotechsual/busybar-dotnet)
[![License: MIT](https://img.shields.io/github/license/homotechsual/busybar-dotnet?style=for-the-badge&label=License)](https://mit.licenses.homotechsual.dev)
[![GitHub Stars](https://img.shields.io/github/stars/homotechsual/busybar-dotnet?style=for-the-badge&label=Stars)](https://github.com/homotechsual/busybar-dotnet/stargazers)
[![Docs](https://img.shields.io/badge/docs-dotnet.busy.homotechsual.dev-a42828?style=for-the-badge)](https://dotnet.busy.homotechsual.dev)

A typed .NET client for the [BUSY Bar HTTP API](https://docs.busy.app/bar/dev/http-api),
modelled on the official [`@busy-app/busy-lib`](https://github.com/busy-app/busylib-ts)
TypeScript client. Full docs, including the API reference, live at
[dotnet.busy.homotechsual.dev](https://dotnet.busy.homotechsual.dev).

## Install

```bash
dotnet add package BusyBar
```

## Quick start

```csharp
using Busy.Bar;

var bar = new BusyBar(new BusyBarOptions { Addr = "10.0.4.20" });

var status = await bar.SystemStatusGetAsync();

await bar.DisplayDrawAsync(new DisplayDrawParams
{
    ApplicationName = "my_app",
    Elements = new DisplayElement[]
    {
        new TextElement { Id = "0", Text = "Hello!", Font = TextFont.Normal, Align = ElementAlign.Center }
    }
});
```

See [Getting Started](https://dotnet.busy.homotechsual.dev/intro) for scope, local-vs-cloud
addressing, and [error handling](https://dotnet.busy.homotechsual.dev/intro#error-handling).

## Development transparency

AI tools (Claude Code) are used as productivity assistants in this project's development. All
AI-assisted code is reviewed by a human before it is pushed or published, and release gates and
processes are built around human reviewers — nothing merges, ships, or gets tagged as a release
without human sign-off.

## License

[MIT](LICENSE)
