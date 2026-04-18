using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using WebsiteAIAssistant;
using WebsiteAIAssistant.AWSLambda;

namespace SampleWebsite.AWSLambda
{
    public class TestExecutor : BackgroundService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<TestExecutor>? _logger;
        private CancellationTokenSource? _cts;
        private Task? _backgroundTask;

        private readonly static TestLambdaContext testContext = new TestLambdaContext();
        private readonly static PredictionLambda aiAssistant = new PredictionLambda();
        private readonly static Func<string, ILambdaContext, Task<object>> aiAssistantLambdaHandler = aiAssistant.GetHandler;

        public TestExecutor(IHostApplicationLifetime appLifetime, ILogger<TestExecutor>? logger = null)
        {
            _appLifetime = appLifetime;
            _logger = logger;
        }

        public async Task ExecuteAsync(string userInput)
        {
            var result = await aiAssistantLambdaHandler(userInput, testContext);
            
            Console.WriteLine($"---------------------------------------------------------");
            Console.WriteLine($"Input: {userInput}");
            PrintPrediction((Prediction)result);
            Console.WriteLine($"---------------------------------------------------------");
        }

        private static void PrintPrediction(Prediction prediction)
        {
            var predictedScheme = (Scheme)float.Parse(prediction.PredictedLabel);

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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }

            if (_backgroundTask != null)
            {                
                await _backgroundTask;
            }
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _appLifetime.ApplicationStarted.Register(async () =>
            {
                Console.WriteLine("Application has started. Waiting for 10 seconds before executing tests...");
                await Task.Delay(10 * 1000, cancellationToken);
                _backgroundTask = RunTests(_cts.Token);
                await _backgroundTask;
                _backgroundTask = null;
            });
        }

        private async Task RunTests(CancellationToken cancellationToken)
        {
            Console.WriteLine($"=============== Running tests ===============");
            Console.WriteLine(Environment.NewLine);

            //ACCU
            await this.ExecuteAsync("What are the requisites for carbon credits?");

            //ACCU
            await this.ExecuteAsync("how do I change my business techniques to enhance efficiency and lower emissions?");

            await this.ExecuteAsync("how do I alter my establishment's processes to improve effectiveness and reduce emissions?");

            //NGER
            await this.ExecuteAsync("I want to report on my emissions");

            //NGER
            await this.ExecuteAsync("what is my mandatory emission reporting?");

            //NGER
            await this.ExecuteAsync("how do I calculate net emissions?");

            //Safeguard Mechanism
            await this.ExecuteAsync("how are emission baselines calculated?");

            //Invalid search
            await this.ExecuteAsync("987 abc");

            //Invalid search
            await this.ExecuteAsync("xyz abc pqr");

            //Invalid search
            await this.ExecuteAsync("what is the colour of a rose?");

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"=============== End of tests ===============");
        }
    }
}
