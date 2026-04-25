using Microsoft.ML.Data;

namespace WebsiteAIAssistant.Tests
{
    public class ModelInputExtended : ModelInput
    {
        [LoadColumn(2)]
        public string Class { get; set; } = string.Empty;
        [LoadColumn(3)]
        public string Range { get; set; } = string.Empty;
        [LoadColumn(4)]
        public string Price { get; set; } = string.Empty;
    }
}
