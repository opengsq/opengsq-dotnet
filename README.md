# OpenGSQ .NET Library
[![Dotnet Package](https://github.com/opengsq/opengsq-dotnet/actions/workflows/dotnet-package.yml/badge.svg)](https://github.com/opengsq/opengsq-dotnet/actions/workflows/dotnet-package.yml)
[![GitHub license](https://img.shields.io/github/license/opengsq/opengsq-dotnet)](https://github.com/opengsq/opengsq-dotnet/blob/main/LICENSE)
[![NuGet Version](http://img.shields.io/nuget/v/OpenGSQ.svg?style=flat)](https://www.nuget.org/packages/OpenGSQ/)
![NuGet Downloads](https://img.shields.io/nuget/dt/OpenGSQ)

The OpenGSQ .NET library provides a convenient way to query servers from applications written in the C# language.

## Documentation

Detailed documentation is available at [https://dotnet.opengsq.com](https://dotnet.opengsq.com).

## Supported Protocols

A list of supported protocols can be found at [https://dotnet.opengsq.com/api/OpenGSQ.Protocols.html](https://dotnet.opengsq.com/api/OpenGSQ.Protocols.html).

## Prerequisities

The library requires a minimum of .NET Standard 2.0.

You can find a list of all supported frameworks at [Supported Frameworks](https://www.nuget.org/packages/OpenGSQ/#supportedframeworks-body-tab).

## Installation

You can find the package through the NuGet Package Manager or install it using the following command:

```
dotnet add package OpenGSQ
```

## Usage

Here is an example of how to use the Source Query Protocol with OpenGSQ

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

You can find information about tests and results at [https://dotnet.opengsq.com/tests](https://dotnet.opengsq.com/tests/OpenGSQ.Protocols.Tests/OpenGSQ.Protocols.Tests.html).
