using System;
using System.Collections.Generic;

namespace WebsiteAIAssistant
{
    public class WebsiteAIAssistantSettings
    {
        private float _negativeConfidenceThreshold = 0.70f;

        public float NegativeConfidenceThreshold
        {
            get => _negativeConfidenceThreshold;
            set => _negativeConfidenceThreshold = (value >= 0 && value <= 1) ? value
                                            : throw new ArgumentOutOfRangeException(nameof(NegativeConfidenceThreshold), "NegativeConfidenceThreshold must be between 0 and 1.");
        }
        public float NegativeLabel { get; set; } = -1f;       
        public string AIModelLoadFilePath { get; set; } = string.Empty;
    }

    public class WebsiteAIAssistantCreateModelSettings
    {
        public DataViewType DataViewType { get; set; } = DataViewType.File;
        public string DataViewFilePath { get; set; }
        public IEnumerable<ModelInput> DataViewList { get; set; }
        public string[] StopWords { get; set; } = null;
        public TextFeaturizingEstimatorOptions TextFeaturizingEstimatorOptions { get; set; } = null;
        public SdcaMaximumEntropyOptions SdcaMaximumEntropyOptions { get; set; } = null;
        public string[] ExtendedColumnNames { get; set; } = null;
        public string AIModelFilePath { get; set; } = string.Empty;
    }
}
