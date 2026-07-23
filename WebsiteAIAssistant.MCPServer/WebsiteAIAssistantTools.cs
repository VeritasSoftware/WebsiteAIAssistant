using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WebsiteAIAssistant.MCPServer
{
    [McpServerToolType]
    public class WebsiteAIAssistantTools
    {
        [McpServerTool, Description("Get prediction.")]
        public static async Task<Prediction> GetPrediction(IWebsiteAIAssistantService websiteAIAssistantService,
                                                            [Description("The user input")] string input)
        {
            var model = new ModelInput
            {
                Feature = input
            };

            return await websiteAIAssistantService.PredictAsync(model);
        }
    }
}
