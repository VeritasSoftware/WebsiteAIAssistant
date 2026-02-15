using Microsoft.AspNetCore.Mvc;

namespace WebsiteAIAssistant.MinimalAPI
{
    public static class MinimalAPIExtensions
    {
        public static RouteHandlerBuilder MapWebsiteAIAssistant(this IEndpointRouteBuilder app)
        {
            return app.MapGet("/ai/{input}", async (HttpRequest request, string input, [FromServices] ILogger? logger = null, [FromServices] IPostPredictionService? postPredictionService = null) =>
            {
                logger?.LogInformation("Received input: {Input}", input);

                if (string.IsNullOrWhiteSpace(input))
                {
                    logger?.LogWarning("Input is empty or whitespace.");

                    return Results.BadRequest("Input cannot be empty.");
                }

                logger?.LogInformation("Processing input: {Input}", input);

                var modelInput = new ModelInput { Feature = input.Trim() };

                var prediction = await PredictionEngine.PredictAsync(modelInput);

                if (postPredictionService == null)
                {
                    logger?.LogInformation("No post-prediction service configured. Returning raw prediction.");

                    return Results.Ok(prediction);
                }

                logger?.LogInformation("Post-prediction service configured. Processing prediction with post-prediction service.");

                var result = await postPredictionService.HandlePredictionAsync(modelInput, prediction);

                logger?.LogInformation("Post-prediction service processed the prediction. Returning result.");

                return Results.Ok(result);
            });
        }
    } 
}
