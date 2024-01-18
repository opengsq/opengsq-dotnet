# OpenGSQ .NET Library
[![Dotnet Package](https://github.com/opengsq/opengsq-dotnet/actions/workflows/dotnet-package.yml/badge.svg)](https://github.com/opengsq/opengsq-dotnet/actions/workflows/dotnet-package.yml)
[![GitHub license](https://img.shields.io/github/license/opengsq/opengsq-dotnet)](https://github.com/opengsq/opengsq-dotnet/blob/main/LICENSE)
[![NuGet Version](http://img.shields.io/nuget/v/OpenGSQ.svg?style=flat)](https://www.nuget.org/packages/OpenGSQ/)
![NuGet Downloads](https://img.shields.io/nuget/dt/OpenGSQ)

The OpenGSQ .NET library provides a convenient way to query servers from applications written in the C# language.

## Supported Protocols
```cs
using OpenGSQ.Protocols;

var ase = new ASE("79.137.97.3", 22126);
var battlefield = new Battlefield("94.250.199.214", 47200, 10000);
var doom3 = new Doom3("178.162.135.83", 27735);
var eos = new EOS("5.62.115.46", 7783, 5000, clientId, clientSecret, deploymentId);
var fivem = new FiveM("144.217.10.12", 30120);
var gameSpy1 = new GameSpy1("139.162.235.20", 7778);
var gameSpy2 = new GameSpy2("108.61.236.22", 23000);
var gameSpy3 = new GameSpy3("95.172.92.116", 29900);
var gameSpy4 = new GameSpy4("play.avengetech.me", 19132);
var killingFloor = new KillingFloor("104.234.65.235", 7708);
var minecraft = new Minecraft("valistar.site", 25565);
var quake1 = new Quake1("35.185.44.174", 27500);
var quake2 = new Quake2("46.165.236.118", 27910);
var quake3 = new Quake3("108.61.18.110", 27960);
var raknet = new RakNet("mc.advancius.net", 19132);
var samp = new Samp("51.254.178.238", 7777);
var satisfactory = new Satisfactory("79.136.0.124", 15777);
var scum = new Scum("15.235.181.19", 7042);
var source = new Source("45.62.160.71", 27015);
var teamSpeak3 = new TeamSpeak3("145.239.200.2", 10011, 9987);
var unreal2 = new Unreal2("109.230.224.189", 6970);
var vcmp = new Vcmp("51.178.65.136", 8114);
var won = new WON("212.227.190.150", 27020);
```

## Installation

Find the package through NuGet Package Manager or install it with following command.

```
dotnet add package OpenGSQ
```

## Usage

Source Query Protocol

```cs
using System;
using System.Threading.Tasks;
using OpenGSQ.Protocols;

class Program
{
    static async Task Main()
    {
        // Create a new Source object
        var source = new Source("45.62.160.71", 27015);

        // Call the GetInfo method
        var info = await source.GetInfo();

        // Now you can use the 'info' object
    }
}
```

## Tests and Results

See [OpenGSQTests/Protocols](/OpenGSQTests/Protocols) for the tests.

See [OpenGSQTests/Results](/OpenGSQTests/Results) for tests outputs.
