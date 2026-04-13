namespace WebsiteAIAssistant
{
    public class SdcaMaximumEntropyOptions
    {
        public float BiasLearningRate { get; set; }
        public int? ConvergenceCheckFrequency { get; set; }
        public int? MaximumNumberOfIterations { get; set; }
        public float ConvergenceTolerance { get; set; }
        public float? L1Regularization { get; set; }
        public float? L2Regularization { get; set; }
        public bool Shuffle { get; set; }
        public int? NumberOfThreads { get; set; }

        public override string ToString()
        {
            return $"{nameof(SdcaMaximumEntropyOptions)}: {nameof(BiasLearningRate)}={BiasLearningRate}, {nameof(ConvergenceCheckFrequency)}={ConvergenceCheckFrequency}, {nameof(MaximumNumberOfIterations)}={MaximumNumberOfIterations}, {nameof(ConvergenceTolerance)}={ConvergenceTolerance}, {nameof(L1Regularization)}={L1Regularization}, {nameof(L2Regularization)}={L2Regularization}, {nameof(Shuffle)}={Shuffle}, {nameof(NumberOfThreads)}={NumberOfThreads}";
        }
    }
}
