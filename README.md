# AppleCIDR

[![C#](https://img.shields.io/badge/C%23-.NET_8-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows%20Desktop-0078D4)](https://learn.microsoft.com/dotnet/desktop/wpf/)
[![Built with Codex](https://img.shields.io/badge/Built%20with-Codex-111827)](https://openai.com/codex)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![GitHub Stars](https://img.shields.io/github/stars/Penguin-Dev93/AppleCIDR?style=social)](https://github.com/Penguin-Dev93/AppleCIDR/stargazers)
[![Buy Me A Coffee](https://img.shields.io/badge/Buy%20me%20a%20coffee-%E2%98%95-yellow?style=flat&logo=buy-me-a-coffee&logoColor=black)](https://buymeacoffee.com/penguin.dev93)
[![Follow on X](https://img.shields.io/badge/Follow-%40Penguin__Dev93-1DA1F2?style=flat&logo=twitter&logoColor=white)](https://x.com/Penguin_Dev93)

AppleCIDR is a lightweight Windows desktop subnet calculator for network engineers, systems administrators, and IT professionals. It has one job: calculate IPv4 subnet details from an IP address and CIDR prefix.

The app is a focused WPF/.NET utility with inline validation, immediate recalculation, a CIDR slider, and a simple copy-summary action. It does not store user data, call external services, scan networks, or include telemetry.

## Requirements

- Windows 10 or later
- .NET 8 SDK for local builds
- Visual Studio 2022 or the .NET CLI for development

## Usage

1. Launch AppleCIDR.
2. Enter a valid IPv4 address.
3. Adjust the CIDR slider from `/0` through `/32`.
4. Review the calculated subnet mask, wildcard mask, usable host count, network address, first host, last host, and broadcast address.
5. Use `Copy Summary` to copy the current valid calculation.

Invalid IPv4 input shows inline validation and disables copy behavior until the input is valid.

## Build Instructions

Restore and build the WPF project:

```powershell
dotnet restore AppleCIDR/AppleCIDR.csproj
dotnet build AppleCIDR/AppleCIDR.csproj -c Release
```

Create a Windows x64 executable locally:

```powershell
dotnet publish AppleCIDR/AppleCIDR.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish
```

The published executable is `AppleCIDR.exe`.

## GitHub Actions Release

The `.github/workflows/build-release.yml` workflow builds a self-contained Windows x64 executable on `windows-latest`.

Run it manually with `workflow_dispatch`, or push a version tag such as:

```powershell
git tag v1.0.0
git push origin v1.0.0
```

Tagged builds upload `AppleCIDR.exe` to a GitHub release.
