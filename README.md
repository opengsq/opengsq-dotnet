# OpenGSQ .NET Library

The OpenGSQ .NET library provides a convenient way to query servers from applications written in the C# language.

## Installation

Find the package through NuGet Package Manager or install it with following command.

```
dotnet add package OpenGSQ
```

## Usage

Source Query Protocol

```cs
using System;
using OpenGSQ.Protocols;

namespace OpenGSQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Source source = new Source("91.216.250.14", 27015);

            // Get Server Info
            Source.IResponse response = source.GetInfo();
           
            Console.WriteLine(response.Name); // skial.com | SAXTON HALE | US ███
            Console.WriteLine(response.Map);  // vsh_dust_showdown_deluxe_r1

            // If you want to get full response, try cast to SourceResponse or GoldSourceResponse
            if (response is Source.SourceResponse sourceResponse)
            {
                // Casted from IResponse to SourceResponse

                Console.WriteLine(sourceResponse.ID); // 440
            }
            else if (response is Source.GoldSourceResponse goldSourceResponse)
            {
                // Casted from IResponse to GoldSourceResponse

                Console.WriteLine(goldSourceResponse.Address);
            }
        }
    }
}
```

Source RCON Protocol
```cs
using System;
using OpenGSQ.Protocols;

namespace OpenGSQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using var remoteConsole = new Source.RemoteConsole("", 27015);

            try
            {
                // Authenticate with rcon password
                remoteConsole.Authenticate("");

                // Send command and receive the response
                string response = remoteConsole.SendCommand("cvarlist");

                Console.WriteLine(response);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }
    }
}
```

## Supported Protocols
```cs
var gameSpy1 = new GameSpy1("123.123.123.123", 7778);
var gameSpy2 = new GameSpy2("123.123.123.123", 23000);
var gameSpy3 = new GameSpy3("123.123.123.123", 29900);
var gameSpy4 = new GameSpy4("123.123.123.123", 19132);
var quake1 = new Quake1("123.123.123.123", 27500);
var quake2 = new Quake2("123.123.123.123", 27910);
var quake3 = new Quake3("123.123.123.123", 27960);
var source = new Source("123.123.123.123", 27015); // Both Source and Goldsource supported
```

See [OpenGSQTests/Protocols](/OpenGSQTests/Protocols) for the tests.

See [OpenGSQTests/Results](/OpenGSQTests/Results) for tests outputs.
