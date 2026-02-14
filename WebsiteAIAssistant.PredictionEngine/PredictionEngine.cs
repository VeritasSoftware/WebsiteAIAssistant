using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteAIAssistant.PredictionEngine
{
    public enum DataViewType
    {
        File,
        List
    }

    public static class PredictionEngine
    {
        private static PredictionEngine<ModelInput, Prediction> _predictionEngine;

        public static DataViewType DataViewType { get; set; } = DataViewType.File;
        public static string DataViewFilePath { get; set; }
        public static IEnumerable<ModelInput> DataViewList { get; set; }       

        public static async Task CreateModelAsync(string modelPath)
        {
            var mlContext = new MLContext();

            IDataView dataView;

            if (DataViewType == DataViewType.File)
            {
                dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                path: DataViewFilePath,
                hasHeader: false,
                separatorChar: '\t');
            }
            else
            {
                dataView = mlContext.Data.LoadFromEnumerable(DataViewList);
            }

            string[] stopWords = new[] { "the", "a", "an", "is", "and", "or", "of", "to", "in" };

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
                MaximumNumberOfIterations = 500,       // More passes for better convergence
                ConvergenceTolerance = 1e-5f,          // Stricter tolerance
                L1Regularization = 1e-4f,              // Small L1 penalty
                L2Regularization = 1e-4f,              // Small L2 penalty
                Shuffle = true
            };

            var trainingPipeline = pipeline
                                    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(options))
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                                                        outputColumnName: nameof(Prediction.PredictedLabel),
                                                        inputColumnName: nameof(Prediction.PredictedLabel)));

            var model = trainingPipeline.Fit(dataView);           

            // Save model with schema
            mlContext.Model.Save(model, dataView.Schema, modelPath);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, Prediction>(model);

            _predictionEngine = predictionEngine;

            await Task.CompletedTask;
        }

        public static async Task LoadModelAsync(string modelPath)
        {
            //Define DataViewSchema for data preparation pipeline and trained model
            DataViewSchema modelSchema;

            var mlContext = new MLContext();

            // Load trained model
            ITransformer trainedModel = mlContext.Model.Load(modelPath, out modelSchema);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, Prediction>(trainedModel);

            _predictionEngine = predictionEngine;

            await Task.CompletedTask;
        }

        public static async Task<Prediction> PredictAsync(ModelInput input)
        {
            if (_predictionEngine == null)
            {
                throw new InvalidOperationException("Prediction engine is not initialized. Please create & load the model first.");
            }

            var prediction = _predictionEngine.Predict(input);

            if (prediction.PredictedLabel < 0)
            {
                if (prediction.Score[0] < 0.70)
                {
                    var secondHighestScore = prediction.Score
                                                        .OrderByDescending(s => s)
                                                        .Skip(1)
                                                        .First();
                    var index = Array.IndexOf(prediction.Score, secondHighestScore);

                    prediction.PredictedLabel = index - 1;
                }
            }

            return await Task.FromResult(prediction);
        }
    }
}
