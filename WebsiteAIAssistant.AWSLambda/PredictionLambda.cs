using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WebsiteAIAssistant.AWSLambda;

public class PredictionLambda
{
    private readonly ILogger<PredictionLambda>? _logger;
    private readonly IPostPredictionService? _postPredictionService;

    public PredictionLambda()
    {
    }

    public PredictionLambda(IPostPredictionService? postPredictionService = null,
                                        ILogger<PredictionLambda>? logger = null)
    {
        _postPredictionService = postPredictionService;
        _logger = logger;
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/ai/{input}")]
    public async Task<object> GetHandler(string input, ILambdaContext context)
    {
        LogInformation(context, string.Format("Received input: {0}", input));

        input = input?.Trim() ?? string.Empty;        

        if (string.IsNullOrWhiteSpace(input))
        {
            LogInformation(context, "Input is empty or whitespace.");

            return "Input cannot be empty.";
        }

        LogInformation(context, string.Format("Processing input: {0}", input));

        var modelInput = new ModelInput { Feature = input.Trim() };

        var prediction = await PredictionEngine.PredictAsync(modelInput);

        if (_postPredictionService == null)
        {
            LogInformation(context, "No post-prediction service configured. Returning raw prediction.");

            return prediction;
        }

        LogInformation(context, "Post-prediction service configured. Processing prediction with post-prediction service.");

        var result = await _postPredictionService.HandlePredictionAsync(context, modelInput, prediction);

        LogInformation(context, "Post-prediction service processed the prediction. Returning result.");

        return result;
    }

    private void LogInformation(ILambdaContext context, string message, params object[] args)
    {
        _logger?.LogInformation(message, args);
        context.Logger.LogInformation(message, args);
    }
}
