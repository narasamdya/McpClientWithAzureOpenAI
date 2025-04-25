# McpClientWithAzureOpenAI

This is an example of MCP client using Azure OpenAI chat client. This example also uses Microsoft.Extensions.AI for implementation.

## Running the client

Set API key:
```shell
dotnet user-secrets init
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "<your key here>"
```

Run client
```shell
dotnet run -- "<path to your server>"
```
e.g., for [McpWeatherServer](https://github.com/narasamdya/McpWeatherServer)
```shell
dotnet run -- McpWeatherServer/WeatherServer.csproj
```

## Resources

* [Unified AI building blocks for .NET using Microsoft.Extensions.AI](https://learn.microsoft.com/en-us/dotnet/ai/ai-extensions)
* [MCP for client developer](https://modelcontextprotocol.io/quickstart/client#c)
* [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
* [Microsoft.Extensions.AI](https://github.com/dotnet/extensions/blob/main/src/Libraries/Microsoft.Extensions.AI/README.md)
