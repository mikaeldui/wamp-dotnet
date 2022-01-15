
# Mikael Dúi's [WAMP][wamp] Library for .NET
[![.NET](https://github.com/mikaeldui/wamp-dotnet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mikaeldui/wamp-dotnet/actions/workflows/dotnet.yml)
[![CodeQL Analysis](https://github.com/mikaeldui/wamp-dotnet/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/mikaeldui/wamp-dotnet/actions/workflows/codeql-analysis.yml)

![image](https://user-images.githubusercontent.com/3706841/149625415-709dd31c-425d-4421-9d60-5eb4757f83bd.png)


Adds [WAMP][wamp] classes to `System.Net.WebSockets.Wamp`.

You can install it using the following **.NET CLI** command:

    dotnet add package MikaelDui.Net.WebSockets.Wamp --version *
    
    
## Example

    using System.Net.WebSockets.Wamp;
    
    using WampSubscriberClient client = new()
    await client.OpenAsync("wss://example.com/");
    await client.SubscribeAsync("news");
    var newsMessage = await client.ReceiveAsync();
    Console.WriteLines("News received: " newsMessage.Elements[0].GetString());
    await client.CloseAsync();

[wamp]: https://github.com/wamp-proto/wamp-proto
