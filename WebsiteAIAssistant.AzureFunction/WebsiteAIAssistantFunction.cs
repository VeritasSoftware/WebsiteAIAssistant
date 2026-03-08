using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebsiteAIAssistant.AzureFunction
{
    public class WebsiteAIAssistantFunction
    {
        private readonly ILogger<WebsiteAIAssistantFunction>? _logger;
        private readonly IPostPredictionService? _postPredictionService;

        public WebsiteAIAssistantFunction(IPostPredictionService? postPredictionService = null, ILogger<WebsiteAIAssistantFunction>? logger = null)
        {
            _postPredictionService = postPredictionService;
            _logger = logger;
        }

        [Function("WebsiteAIAssistant-GetPrediction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ai/{input}")]
                                    HttpRequest req, string input)
        {
            input = input?.Trim() ?? string.Empty;
            _logger?.LogInformation("Received input: {0}", input);

            if (string.IsNullOrWhiteSpace(input))
            {
                _logger?.LogWarning("Input is empty or whitespace.");

                return new BadRequestObjectResult("Input cannot be empty.");
            }

            _logger?.LogInformation("Processing input: {0}", input);

            var modelInput = new ModelInput { Feature = input.Trim() };

            var prediction = await PredictionEngine.PredictAsync(modelInput);

            if (_postPredictionService == null)
            {
                _logger?.LogInformation("No post-prediction service configured. Returning raw prediction.");

                return new OkObjectResult(prediction);
            }

            _logger?.LogInformation("Post-prediction service configured. Processing prediction with post-prediction service.");

            var result = await _postPredictionService.HandlePredictionAsync(req, modelInput, prediction);

            _logger?.LogInformation("Post-prediction service processed the prediction. Returning result.");

            return new OkObjectResult(result);
        }
    }
}
