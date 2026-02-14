namespace SampleWebsite.PredictionEngine
{
    public static class PredictionEngine
    {
        public static async void SetDataviewPath(string dataViewPath)
        {
            WebsiteAIAssistant.PredictionEngine.PredictionEngine.DataViewPath = dataViewPath;
        }
    }
}
