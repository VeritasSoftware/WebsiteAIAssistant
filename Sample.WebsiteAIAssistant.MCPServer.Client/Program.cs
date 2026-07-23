using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Text.Json;

Console.WriteLine("Client for Website AI Assistant MCP Server...");
Console.WriteLine(Environment.NewLine);

var transport = new StdioClientTransport(new()
{
    Command = "dotnet run",
    Arguments = ["--project", @"..\..\..\..\Sample.WebsiteAIAssistant.MCPServer"],
    Name = "Minimal MCP Server",
});
McpClient mcpClient = await McpClient.CreateAsync(transport);

// List all available tools from the MCP server.
Console.WriteLine("Available tools:");
Console.WriteLine(Environment.NewLine);
IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
foreach (McpClientTool tool in tools)
{
    Console.WriteLine($"{tool}");
}

// Valid input
await CallToolAsync(mcpClient, "What are the requisites for carbon credits?");

await CallToolAsync(mcpClient, "How do I calculate net emissions?");

// Invalid input
await CallToolAsync(mcpClient, "What is the colour of a rose?");

await mcpClient.DisposeAsync();

Console.ReadLine();

static async Task CallToolAsync(McpClient mcpClient, string input)
{
    var param = new CallToolRequestParams
    {
        Name = "get_prediction",
        Arguments = new Dictionary<string, JsonElement>
    {
        { "input", JsonSerializer.SerializeToElement(input) }
    }
    };

    var result = await mcpClient.CallToolAsync(param);

    Console.WriteLine(Environment.NewLine);
    Console.WriteLine($"Tool input: {input}");
    Console.WriteLine($"Tool result: {result.Content[0]}");
}