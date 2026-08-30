using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebsiteAIAssistant.MCPServer.Client
{
    public class Content
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("content")]
        public List<Content> Content { get; set; }
    }

    public class ServerResult
    {
        [JsonPropertyName("result")]
        public Result Result { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }
    }
}
