namespace WebsiteAIAssistant
{
    public class TextFeaturizingEstimatorOptions
    {
        public TextFeaturizingEstimatorCaseMode CaseMode { get; set; }
        public bool KeepDiacritics { get; set; } = true;
        public bool KeepPunctuations { get; set; } = true;
        public bool KeepNumbers { get; set; } = true;
        public WordBagEstimatorOptions CharFeatureExtractor { get; set; } = null;
        public WordBagEstimatorOptions WordFeatureExtractor { get; set; } = null;
    }

    public enum TextFeaturizingEstimatorCaseMode
    {
        Lower = 0,
        Upper = 1,
        None = 2
    }

    public class WordBagEstimatorOptions
    {
        public int NgramLength { get; set; } = 1;
        public bool UseAllLengths { get; set; } = true;
        public int[] MaximumNgramsCount { get; set; }
        public int SkipLength { get; set; }
        public WordBagWeightingCriteria Weighting { get; set; }
    }

    public enum WordBagWeightingCriteria
    {
        //     Term Frequency. Calculated based on the number of occurrences in the document.
        Tf,
        //
        // Summary:
        //     Inverse Document Frequency. A ratio (the logarithm of inverse relative frequency)
        //     that measures the information a slot provides by determining how common or rare
        //     it is across the entire corpus.
        Idf,
        //
        // Summary:
        //     The product of the term frequency and the inverse document frequency.
        TfIdf
    }
}
