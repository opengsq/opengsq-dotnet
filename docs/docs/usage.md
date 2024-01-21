# Usage

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
