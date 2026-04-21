using Microsoft.ML.Data;

namespace WebsiteAIAssistant.Tests
{
    public class ModelInputExtended : ModelInput
    {
        [LoadColumn(2)]
        public string Feature1 { get; set; } = string.Empty;
        [LoadColumn(3)]
        public string Feature2 { get; set; } = string.Empty;
        [LoadColumn(4)]
        public string Feature3 { get; set; } = string.Empty;
    }
}
