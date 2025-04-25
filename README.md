# McpClientWithAzureOpenAI
This is an example of MCP client using Azure OpenAI chat client

Set API key:
```shell
dotnet user-secrets init
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "<your key here>"
```

Run client
```shell
dotnet run -- "<path to your server>"Q:\src\MCPLearn\WeatherServer\WeatherServer.csproj
```
e.g., for [McpWeatherServer](https://github.com/narasamdya/McpWeatherServer)
```shell
dotnet run -- McpWeatherServer/WeatherServer.csproj
```
