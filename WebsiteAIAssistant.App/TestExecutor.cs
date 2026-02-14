using AspNetCore.WebsiteAIAssistant;

namespace WebsiteAIAssistant.App
{
    public static class TestExecutor
    {
        public static async Task CreateModelAsync()
        {
            // Path to save model
            string modelPath = Path.Combine(Environment.CurrentDirectory, "SampleWebsite-AI-Model.zip");

            await PredictionEngine.CreateModelAsync(modelPath);

            Console.WriteLine("AI model created successfully...");
        }


        public static async Task CreatePredictionEngineAsync()
        {
            await PredictionEngine.LoadModelAsync("SampleWebsite-AI-Model.zip");

            Console.WriteLine("AI Prediction Engine created successfully...");
        }

        public static async Task ExecuteAsync(string userInput)
        {
            var input = new ModelInput { Feature = userInput };

            var prediction = await PredictionEngine.PredictAsync(input);

            Console.WriteLine($"---------------------------------------------------------");
            Console.WriteLine($"Input: {input.Feature}");
            PrintPrediction(prediction);
        }

        public static void PrintPrediction(Prediction prediction)
        {
            var predictedScheme =  (Scheme)prediction.PredictedLabel;

            Console.WriteLine($"Predicted Scheme: {predictedScheme.ToString()}");

            Console.WriteLine($"Score: ");

            if (prediction.Score != null)
            {
                foreach (var score in prediction.Score)
                {
                    Console.WriteLine($"{score}");
                }
            }
        }
    }
}
