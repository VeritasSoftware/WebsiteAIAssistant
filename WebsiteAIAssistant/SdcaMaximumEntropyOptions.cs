namespace WebsiteAIAssistant
{
    public class SdcaMaximumEntropyOptions
    {
        public float BiasLearningRate { get; set; }
        public int? ConvergenceCheckFrequency { get; set; }
        public int MaximumNumberOfIterations { get; set; }
        public float ConvergenceTolerance { get; set; }
        public float L1Regularization { get; set; }
        public float L2Regularization { get; set; }
        public bool Shuffle { get; set; }

    }
}
