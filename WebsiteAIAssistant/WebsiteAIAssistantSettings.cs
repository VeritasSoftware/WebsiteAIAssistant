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
        public string AIModelFilePath { get; set; } = string.Empty;
    }

    public class WebsiteAIAssistantCreateModelSettings
    {
        private float _negativeConfidenceThreshold = 0.70f;

        public DataViewType DataViewType { get; set; } = DataViewType.File;
        public string DataViewFilePath { get; set; }
        public IEnumerable<ModelInput> DataViewList { get; set; }       
        public string AIModelFilePath { get; set; } = string.Empty;
    }
}
