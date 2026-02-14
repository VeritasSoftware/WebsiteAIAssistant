using Microsoft.ML.Data;

namespace AspNetCore.WebsiteAIAssistant
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public float Label { get; set; }

        [LoadColumn(1)]
        public string Feature { get; set; }
    }
}
