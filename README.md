# OpenGSQ .NET Library

The OpenGSQ .NET library provides a convenient way to query servers from applications written in the C# language.

## Installation

Find the package through NuGet Package Manager or install it with following command.

```
dotnet add package OpenGSQ
```

## Usage
Import the OpenGSQ Protocols library.
```cs
using OpenGSQ.Protocols;
```
Supported Protocols
```cs
var gameSpy1 = new GameSpy1("123.123.123.123", 7778);
var gameSpy2 = new GameSpy2("123.123.123.123", 23000);
var gameSpy3 = new GameSpy3("123.123.123.123", 29900);
var gameSpy4 = new GameSpy4("123.123.123.123", 19132);
var quake1 = new Quake1("123.123.123.123", 27500);
var quake2 = new Quake2("123.123.123.123", 27910);
var quake3 = new Quake3("123.123.123.123", 27960);
var source = new Source("123.123.123.123", 27015);
```
Source RCON Protocol
```cs
var rcon = new Source.RemoteConsole("123.123.123.123", 27015);
rcon.Authenticate("serverRconPassword");
string response = rcon.SendCommand("cvarlist");
```

See [OpenGSQTests](/OpenGSQTests/Protocols) for more examples and outputs.

