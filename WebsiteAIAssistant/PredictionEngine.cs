using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteAIAssistant
{   
    public static class PredictionEngine
    {
        private static PredictionEngine<ModelInput, Prediction> _predictionEngine;
        private static float _negativeConfidenceThreshold = 0.70f;

        public static DataViewType DataViewType { get; set; } = DataViewType.File;
        public static string DataViewFilePath { get; set; }
        public static IEnumerable<ModelInput> DataViewList { get; set; }
        public static IWebsiteAIAssistantLogger Logger { get; set; } = null;
        public static float NegativeConfidenceThreshold 
        {
            get => _negativeConfidenceThreshold;
            set => _negativeConfidenceThreshold = ValidateThreshold(value);
        }
        public static float NegativeLabel { get; set; } = -1f;
        public static SdcaMaximumEntropyOptions SdcaMaximumEntropyOptions { get; set; } = new SdcaMaximumEntropyOptions();
        private static float ValidateThreshold(float threshold)
        {
            Logger?.LogInformation($"{nameof(PredictionEngine)}: {threshold}");
            if (threshold >= 0 && threshold <= 1)
            {
                if (threshold < .50f)
                {
                    Logger?.LogWarning($"{nameof(NegativeConfidenceThreshold)} : {threshold} too low. Not enough confidence to classify as negative prediction.");
                }
                else if (threshold > .90f)
                {
                    Logger?.LogWarning($"{nameof(NegativeConfidenceThreshold)} : {threshold} too high. May miss valid negative predictions.");
                }
                return threshold;
            }
            throw new ArgumentOutOfRangeException(nameof(NegativeConfidenceThreshold), $"{nameof(NegativeConfidenceThreshold)} must be between 0 and 1.");
        }        

        public static async Task CreateModelAsync(string modelPath)
        {
            Logger?.LogInformation("Starting model creation process...");
            var mlContext = new MLContext();

            IDataView dataView;

            Logger?.LogInformation("Loading training data using DataViewType: {0}", DataViewType);
            if (DataViewType == DataViewType.File)
            {
                if (string.IsNullOrEmpty(DataViewFilePath))
                {
                    Logger?.LogError($"{nameof(DataViewFilePath)} is null or empty. Cannot load training data.");
                    throw new InvalidOperationException($"{nameof(DataViewFilePath)} is null or empty. Please provide a valid file path to the training data.");
                }

                Logger?.LogInformation("Loading training data from file: {0}", DataViewFilePath);

                dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                path: DataViewFilePath,
                hasHeader: false,
                separatorChar: '\t');

                Logger?.LogInformation("Training data loaded successfully from file with {0} records.", mlContext.Data.CreateEnumerable<ModelInput>(dataView, reuseRowObject: false).Count());
            }
            else
            {
                if (DataViewList == null || !DataViewList.Any())
                {
                    Logger?.LogError($"{nameof(DataViewList)} is null or empty. Cannot load training data.");

                    throw new InvalidOperationException($"{nameof(DataViewList)} is null or empty. Please provide valid data for training.");
                }

                Logger?.LogInformation("Loading training data from in-memory list with {0} records.", DataViewList.Count());
                dataView = mlContext.Data.LoadFromEnumerable(DataViewList);
                Logger?.LogInformation("Training data loaded successfully from in-memory list.");
            }

            string[] stopWords = new[] { "the", "a", "an", "is", "and", "or", "of", "to", "in" };

            Logger?.LogInformation("Building data processing and training pipeline...");
            var pipeline = mlContext.Transforms.Text.TokenizeIntoWords("Tokens", nameof(ModelInput.Feature))
                            .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Tokens", "Tokens")) // Built-in stopwords
                            .Append(mlContext.Transforms.Text.RemoveStopWords("Tokens", stopwords: stopWords)) // Custom stopwords
                            .Append(mlContext.Transforms.Text.FeaturizeText(
                                outputColumnName: "Features",
                                inputColumnName: "Tokens"))                           
                            .Append(mlContext.Transforms.Conversion.MapValueToKey(
                                outputColumnName: "LabelKey",
                                inputColumnName: nameof(ModelInput.Label)))
                            .Append(mlContext.Transforms.DropColumns("Tokens", nameof(ModelInput.Label), nameof(ModelInput.Feature)));

            var options = new SdcaMaximumEntropyMulticlassTrainer.Options
            {
                LabelColumnName = "LabelKey",
                FeatureColumnName = "Features",
                MaximumNumberOfIterations = SdcaMaximumEntropyOptions.MaximumNumberOfIterations,
                ConvergenceTolerance = SdcaMaximumEntropyOptions.ConvergenceTolerance,          
                L1Regularization = SdcaMaximumEntropyOptions.L1Regularization,              
                L2Regularization = SdcaMaximumEntropyOptions.L2Regularization,              
                Shuffle = SdcaMaximumEntropyOptions.Shuffle
            };

            Logger?.LogInformation("Training the model with SdcaMaximumEntropy trainer...");
            var trainingPipeline = pipeline
                                    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(options))
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                                                        outputColumnName: nameof(Prediction.PredictedLabel),
                                                        inputColumnName: nameof(Prediction.PredictedLabel)));

            Logger?.LogInformation("Fitting the model to the training data...");
            var model = trainingPipeline.Fit(dataView);           

            // Save model with schema
            Logger?.LogInformation("Saving the trained model to: {0}", modelPath);
            mlContext.Model.Save(model, dataView.Schema, modelPath);

            Logger?.LogInformation("Model creation and saving process completed successfully.");
            await Task.CompletedTask;
        }

        public static async Task LoadModelAsync(string modelPath)
        {
            Logger?.LogInformation("Loading model from path: {0}", modelPath);
            //Define DataViewSchema for data preparation pipeline and trained model
            DataViewSchema modelSchema;

            var mlContext = new MLContext();

            // Load trained model
            ITransformer trainedModel = mlContext.Model.Load(modelPath, out modelSchema);

            Logger?.LogInformation("Model loaded successfully. Creating prediction engine...");
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, Prediction>(trainedModel);

            _predictionEngine = predictionEngine;

            Logger?.LogInformation("Prediction engine created successfully and ready for predictions.");
            await Task.CompletedTask;
        }

        public static async Task<Prediction> PredictAsync(ModelInput input)
        {
            if (_predictionEngine == null)
            {
                Logger?.LogError("Prediction engine is not initialized. Cannot perform prediction.");
                throw new InvalidOperationException("Prediction engine is not initialized. Please create & load the model first.");
            }

            Logger?.LogInformation("Making prediction for input: {0}", input.Feature);
            var prediction = _predictionEngine.Predict(input);
            Logger?.LogInformation("Raw prediction: PredictedLabel={0}, Scores=[{1}]", prediction.PredictedLabel, string.Join(", ", prediction.Score));

            if (prediction.PredictedLabel <= NegativeLabel)
            {
                Logger?.LogInformation("Negative prediction detected (PredictedLabel={0}). Checking confidence score...", prediction.PredictedLabel);
                if (prediction.Score[0] < NegativeConfidenceThreshold)
                {
                    Logger?.LogInformation("Negative prediction with low confidence (Score[0]={0}). Checking for second highest score...", prediction.Score[0]);
                    var secondHighestScore = prediction.Score
                                                        .OrderByDescending(s => s)
                                                        .Skip(1)
                                                        .First();
                    var index = Array.IndexOf(prediction.Score, secondHighestScore);

                    prediction.PredictedLabel = index - 1;
                }
                Logger?.LogInformation("Final prediction after confidence check: PredictedLabel={0}", prediction.PredictedLabel);
            }

            Logger?.LogInformation("Prediction process completed for input: {0}", input.Feature);

            return await Task.FromResult(prediction);
        }
    }
}
