using System.Text;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();
    
// Create a logger factory
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        // Set the log level to LogLevel.Trace for debugging.
        .SetMinimumLevel(LogLevel.Information) // Set the minimum log level to Debug
        .AddConsole();                         // Add console logging
});

var (command, arguments) = GetCommandAndArguments(args);

var clientTransport = new StdioClientTransport(new()
{
    Name = "Weather server",
    Command = command,
    Arguments = arguments,
});

await using var mcpClient = await McpClientFactory.CreateAsync(
    clientTransport,
    loggerFactory: loggerFactory);

var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}

/// Uncomment below to test the server
// var result = await mcpClient.CallToolAsync(
//     "GetAlerts",
//     new Dictionary<string, object?>() { ["state"] = "WA" });
// Console.WriteLine(string.Join(Environment.NewLine, result.Content.Select(c => c.Text)));

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("MCP Client Started!");
Console.ResetColor();

var endpoint = new Uri("https://ai-imnarasa6641ai971487525928.openai.azure.com/");
var deploymentName = "gpt-4";

var chatOptions = new ChatOptions
{
    Temperature = 0.7f,
    Tools = [..tools]
};

var azureClient = new AzureOpenAIClient(
    endpoint,
    new AzureKeyCredential(builder.Configuration["AZURE_OPENAI_API_KEY"]!));
IChatClient chatClient = azureClient.GetChatClient(deploymentName).AsIChatClient();

IChatClient client = new ChatClientBuilder(chatClient)
    .UseFunctionInvocation()
    .Build();

var chatHistory = new List<ChatMessage>
{
    new(ChatRole.System, "You are an assistant expert in getting weather information in US states"),
};

PromptForInput();
var assistantChat = new StringBuilder(4096);

while(true)
{   
    string? query = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(query))
    {
        PromptForInput();
        continue;
    }

    if (string.Equals(query, "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    chatHistory.Add(new ChatMessage(ChatRole.User, query));

    await foreach (var message in client.GetStreamingResponseAsync(chatHistory, chatOptions))
    {
        Console.Write(message);
        assistantChat.Append(message);
    }

    Console.WriteLine();

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, assistantChat.ToString()));
    assistantChat.Clear();

    PromptForInput();
}


static (string command, string[] arguments) GetCommandAndArguments(string[] args)
{
    return args switch
    {
        [var script] when script.EndsWith(".py") => ("python", args),
        [var script] when script.EndsWith(".js") => ("node", args),
        [var script] when Directory.Exists(script) || (File.Exists(script) && script.EndsWith(".csproj")) => ("dotnet", ["run", "--project", script, "--no-build"]),
        _ => throw new NotSupportedException("An unsupported server script was provided. Supported scripts are .py, .js, or .csproj")
    };
}

static void PromptForInput()
{
    Console.WriteLine("Enter a command (or 'exit' to quit):");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("> ");
    Console.ResetColor();
}
