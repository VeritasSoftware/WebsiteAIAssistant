using Microsoft.ML.Data;

namespace WebsiteAIAssistant
{
    public class Prediction
    {
        public string PredictedLabel { get; set; }

        public float[] Score { get; set; }
    }

    public class ForecastingPrediction
    {
        [ColumnName("Score")]
        public float PredictedLabel { get; set; }
        //public float[] PredictedLabel { get; set; }
    }
}
