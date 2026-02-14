namespace AspNetCore.WebsiteAIAssistant
{
    public class Prediction
    {
        public float PredictedLabel { get; set; }

        public float[] Score { get; set; }
    }
}
