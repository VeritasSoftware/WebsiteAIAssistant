using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WebsiteAIAssistant.MCPServer
{
    [McpServerToolType]
    public class WebsiteAIAssistantTools
    {
        [McpServerTool, Description("Get prediction.")]
        public static async Task<Prediction> GetPrediction([Description("The user input")] string input, 
                                                            IWebsiteAIAssistantService websiteAIAssistantService,                                                            
                                                            ILogger? logger = null)
        {
            logger?.LogInformation("Received input: {0}", input);

            if (string.IsNullOrWhiteSpace(input))
            {
                logger?.LogWarning("Input is empty or whitespace.");

                return null;
            }

            logger?.LogInformation("Processing input: {0}", input);

            var model = new ModelInput
            {
                Feature = input.Trim()
            };

            var prediction = await websiteAIAssistantService.PredictAsync(model);

            logger?.LogInformation($"Prediction: {prediction}");

            return prediction;
        }
    }
}
