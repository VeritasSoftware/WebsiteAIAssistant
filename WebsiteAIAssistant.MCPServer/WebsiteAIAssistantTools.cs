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
        public static async Task<object> GetPrediction([Description("The user input")] string input, 
                                                            IWebsiteAIAssistantService websiteAIAssistantService,
                                                            IPostPredictionService? postPredictionService = null,
                                                            ILogger? logger = null)
        {
            logger?.LogInformation("Received input: {0}", input);

            if (string.IsNullOrWhiteSpace(input))
            {
                logger?.LogWarning("Input is empty or whitespace.");

                return null;
            }

            logger?.LogInformation("Processing input: {0}", input);

            var modelInput = new ModelInput
            {
                Feature = input.Trim()
            };

            var prediction = await websiteAIAssistantService.PredictAsync(modelInput);

            if (postPredictionService == null)
            {
                logger?.LogInformation("No post-prediction service configured. Returning raw prediction.");

                return prediction;
            }

            logger?.LogInformation("Post-prediction service configured. Processing prediction with post-prediction service.");

            var result = await postPredictionService.HandlePredictionAsync(modelInput, prediction);

            logger?.LogInformation("Post-prediction service processed the prediction. Returning result.");

            return result;
        }
    }
}
